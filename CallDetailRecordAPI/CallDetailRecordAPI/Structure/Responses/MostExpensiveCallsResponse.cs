using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;

namespace CallDetailRecordAPI.Structure.Responses
{
    /// <summary>Represents the most expensive calls response.</summary>
    public class MostExpensiveCallsResponse
    {
        /// <summary>The request data.</summary>
        public required MostExpensiveCallsRequest RequestData { get; set; }

        /// <summary>The data.</summary>
        public required IEnumerable<CallDetailRecord> Data { get; set; }
    }
}
