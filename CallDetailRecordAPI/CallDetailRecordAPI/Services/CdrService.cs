using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Extensions;
using CallDetailRecordAPI.Helpers;
using CallDetailRecordAPI.Services.Interfaces;
using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Globalization;

namespace CallDetailRecordAPI.Services
{
    ///<summary>Represents the CDR service.</summary>
    public class CdrService : ICdrService
    {
        ///<summary>The Mongo database.</summary>
        private readonly IMongoDatabase _database;

        ///<summary>The CDR database configuration.</summary>
        private readonly IOptions<CdrDatabaseConfiguration> _options;

        ///<summary>The CDR collection.</summary>
        private readonly IMongoCollection<CallDetailRecord> _cdrCollection;

        ///<summary>Initializes an instance of <see cref="CdrService"/></summary>
        ///<param name="database">The database</param>
        ///<param name="options">The options</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="database"/>
        ///     <paramref name="options"/>
        /// </exception>
        public CdrService(
            IMongoDatabase database,
            IOptions<CdrDatabaseConfiguration> options)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _cdrCollection = _database.GetCollection<CallDetailRecord>(
                _options.Value.CallRecordsCollectionName);
        }

        #region Public Methods

        /// <inheritdoc/>
        public async Task<bool> UploadCallDetailRecordsCsvAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File can't be null or empty", nameof(file));
            }

            using var streamReader = new StreamReader(file.OpenReadStream());
            using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var csvCallDetailRecords = csvReader
                .GetRecords<CsvCallDetailRecord>()
                .ToList();

            var callDetailRecords = csvCallDetailRecords
                .Select(item => item.ToCallDetailRecord())
                .ToList();

            var validDocuments = callDetailRecords
                .Where(item => item != null)
                .ToList();

            if (validDocuments.Any())
            {
                await _cdrCollection.InsertManyAsync(validDocuments);

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<CallDetailRecord> GetCdrByReferenceAsync(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                throw new ArgumentException("The reference can't be null or whitespace", nameof(reference));
            }

            return await _cdrCollection
                .Find(item => item.Reference == reference)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<CallStatistics> GetCallStatisticsAsync(CallStatisticsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateDates(request.StartDate, request.EndDate);

            var filters = CdrHelper.CreateCallStatisticsQueryFilter(
                request.StartDate,
                request.EndDate,
                request.Type);

            var pipelineDefinition = PipelineDefinition<CallDetailRecord, BsonDocument>.Create(
            new IPipelineStageDefinition[]
            {
                PipelineStageDefinitionBuilder.Match(filters),
                PipelineStageDefinitionBuilder.Group<CallDetailRecord, BsonDocument>(
                    new BsonDocument
                    {
                        { "_id", 1 },
                        { "TotalCalls", new BsonDocument("$sum", 1) },
                        { "TotalDuration", new BsonDocument("$sum", "$Duration") }
                    }),
            });

            var result = await _cdrCollection.AggregateAsync(pipelineDefinition);

            var data = await result.FirstOrDefaultAsync();

            long count = 0;
            long totalDuration = 0;

            if (data != null)
            {
                count = data["TotalCalls"].AsInt32;
                totalDuration = data["TotalDuration"].AsInt32;
            }

            return new CallStatistics
            {
                Count = count,
                TotalDuration = totalDuration
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CallDetailRecord>> GetCdrsByCallerIdAsync(
            CdrsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateCallerId(request.CallerId);

            ValidateDates(request.StartDate, request.EndDate);

            var filters = CdrHelper.CreateQueryFilters(
                request.CallerId,
                request.StartDate,
                request.EndDate,
                request.Type);

            return await _cdrCollection
                .Find(filters)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CallDetailRecord>> GetMostExpensiveCallsByCallerIdAsync(
            MostExpensiveCallsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateCallerId(request.CallerId);

            ValidateTake(request.Take);

            ValidateDates(request.StartDate, request.EndDate);

            var filters = CdrHelper.CreateQueryFilters(
                request.CallerId,
                request.StartDate,
                request.EndDate,
                request.Type);

            var sort = Builders<CallDetailRecord>.Sort.Descending(item => item.Cost);

            return await _cdrCollection
                .Find(filters)
                .Sort(sort)
                .Limit(request.Take)
                .ToListAsync();
        }

        #endregion

        #region Private Methods

        /// <summary>Validates the given date range.</summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="startDate"/>
        ///     <paramref name="endDate"/>
        /// </exception>
        private static void ValidateDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Start date cannot be in the future.", nameof(startDate));
            }

            if (endDate > DateTime.UtcNow)
            {
                throw new ArgumentException("End date cannot be in the future.", nameof(endDate));
            }

            if (endDate < startDate)
            {
                throw new ArgumentException("End date must be greater than or equal to start date.", nameof(endDate));
            }

            TimeSpan dateRange = endDate - startDate;
            if (dateRange > TimeSpan.FromDays(30))
            {
                throw new ArgumentException("The date range cannot exceed 30 days.", nameof(endDate));
            }
        }

        /// <summary>Validates the given caller identifier.</summary>
        /// <param name="callerId">The caller identifier (phone number).</param>
        /// <exception cref="ArgumentException"><paramref name="callerId"/></exception>
        private static void ValidateCallerId(string callerId)
        {
            if (string.IsNullOrWhiteSpace(callerId))
            {
                throw new ArgumentException("Caller identifier can't be null or whitespace.", nameof(callerId));
            }
        }

        /// <summary>Validates the take.</summary>
        /// <param name="take">The take.</param>
        /// <exception cref="ArgumentException"><paramref name="take"/></exception>
        private static void ValidateTake(int take)
        {
            if (take < 1)
            {
                throw new ArgumentException("The take value must be 1 or higher.", nameof(take));
            }
        }

        #endregion
    }
}
