namespace CallDetailRecordAPI.Data.Configurations
{
    /// <summary>Represents the CDR database configuration.</summary>
    public class CdrDatabaseConfiguration
    {
        /// <summary>The connection string.</summary>
        public required string ConnectionString { get; set; }

        /// <summary>The database name.</summary>
        public required string DatabaseName { get; set; }

        /// <summary>The call records collection name.</summary>
        public required string CallRecordsCollectionName { get; set; }
    }
}
