using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Extensions;
using CallDetailRecordAPI.Helpers;
using CallDetailRecordAPI.Services.Interfaces;
using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Globalization;

namespace CallDetailRecordAPI.Services
{
    ///<summary>Represents the CDR service.</summary>
    public class CdrService : ICdrService
    {
        ///<summary>The CDR database configuration.</summary>
        private readonly IOptions<CdrDatabaseConfiguration> _options;

        ///<summary>The CDR collection.</summary>
        private readonly IMongoCollection<CallDetailRecord> _cdrCollection;

        ///<summary>Initializes an instance of <see cref="CdrService"/></summary>
        ///<param name="options">The options</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/>
        /// </exception>
        public CdrService(IOptions<CdrDatabaseConfiguration> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var mongoClient = new MongoClient(
            _options.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                _options.Value.DatabaseName);

            _cdrCollection = mongoDatabase.GetCollection<CallDetailRecord>(
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

            await _cdrCollection.InsertManyAsync(callDetailRecords);

            return true;
        }

        /// <inheritdoc/>
        public async Task<CallDetailRecord> GetCdrByReferenceAsync(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                throw new ArgumentException("The reference can't be null or whitespace", nameof(reference));
            }

            return await _cdrCollection
                .Find(cdr => cdr.Reference == reference)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CallDetailRecord>> GetCdrsByCallerIdAsync(CdrsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateDates(request.StartDate, request.EndDate);

            var filters = CdrHelper.CreateQueryFilters(request);

            return await _cdrCollection
                .Find(filters)
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
        private void ValidateDates(DateTime startDate, DateTime endDate)
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

        #endregion
    }
}
