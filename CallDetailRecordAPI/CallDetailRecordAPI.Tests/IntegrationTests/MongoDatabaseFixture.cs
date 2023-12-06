using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Services;
using CallDetailRecordAPI.Structure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CallDetailRecordAPI.Tests.IntegrationTests
{
    public class MongoDatabaseFixture : IDisposable
    {
        public IMongoCollection<CallDetailRecord> CdrCollection { get; }

        public CdrService Service { get; }

        public MongoDatabaseFixture()
        {
            IOptions<CdrDatabaseConfiguration> configuration = Options.Create(new CdrDatabaseConfiguration
            {
                CallRecordsCollectionName = "CallRecordsTest",
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "CallRecordsDbTest"
            });

            var mongoClient = new MongoClient(configuration.Value.ConnectionString);
            var database = mongoClient.GetDatabase(configuration.Value.DatabaseName);
            CdrCollection = database.GetCollection<CallDetailRecord>(configuration.Value.CallRecordsCollectionName);

            Service = new CdrService(database, configuration);
        }

        public void Dispose()
        {
            CdrCollection.Database.DropCollection(CdrCollection.CollectionNamespace.CollectionName);
        }
    }
}
