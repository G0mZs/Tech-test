namespace CallDetailRecordAPI.Structure.Requests
{
    /// <summary>Represents the most expensive calls request.</summary>
    public class MostExpensiveCallsRequest
    {
        ///<summary>The caller identifier (phone number).</summary>
        public required string CallerId { get; set; }

        ///<summary>The number of calls to be shown.</summary>
        public required int Take { get; set; }

        ///<summary>The start date.</summary>
        public required DateTime StartDate { get; set; }

        ///<summary>The end date.</summary>
        public required DateTime EndDate { get; set; }

        ///<summary>The call type.</summary>
        public CallType? Type { get; set; }
    }
}
