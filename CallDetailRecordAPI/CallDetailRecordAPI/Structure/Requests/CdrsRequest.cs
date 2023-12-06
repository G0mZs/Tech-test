namespace CallDetailRecordAPI.Structure.Requests
{
    /// <summary>Represents the CDRs request by caller identifier.</summary>
    public class CdrsRequest
    {
        ///<summary>The caller identifier (phone number).</summary>
        public required string CallerId { get; set; }

        ///<summary>The start date.</summary>
        public required DateTime StartDate { get; set; }

        ///<summary>The end date.</summary>
        public required DateTime EndDate { get; set; }

        ///<summary>The call type.</summary>
        public CallType? Type { get; set; }
    }
}
