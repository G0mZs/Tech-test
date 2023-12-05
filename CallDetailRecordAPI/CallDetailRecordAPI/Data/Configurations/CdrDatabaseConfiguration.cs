namespace CallDetailRecordAPI.Data.Configurations
{
    public class CdrDatabaseConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string CallRecordsCollectionName { get; set; } = string.Empty;
    }
}
