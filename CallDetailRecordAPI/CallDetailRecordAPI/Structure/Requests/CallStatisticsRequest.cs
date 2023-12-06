namespace CallDetailRecordAPI.Structure.Requests
{
    /// <summary>Represents the call statistics request.</summary>
    public class CallStatisticsRequest
    {
        ///<summary>The start date.</summary>
        public required DateTime StartDate { get; set; }

        ///<summary>The end date.</summary>
        public required DateTime EndDate { get; set; }

        ///<summary>The call type.</summary>
        public CallType? Type { get; set; }
    }
}
