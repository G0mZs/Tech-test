using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Services.Interfaces;
using CallDetailRecordAPI.Structure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CallDetailRecordAPI.Services
{
    public class CdrService : ICdrService
    {
        private readonly IMongoCollection<CallDetailRecord> _cdrCollection;

        public CdrService(
            IMongoDatabase database,
            IOptions<CdrDatabaseConfiguration> options)
        {
            _cdrCollection = database.GetCollection<CallDetailRecord>(
                options.Value.CallRecordsCollectionName);
        }
    }
}
