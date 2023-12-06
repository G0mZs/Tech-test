using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;

namespace CallDetailRecordAPI.Structure.Responses
{
    /// <summary>Represents the call statistics response.</summary>
    public class CallStatisticsResponse
    {
        /// <summary>The request data.</summary>
        public required CallStatisticsRequest RequestData { get; set; }

        /// <summary>The data.</summary>
        public required CallStatistics Data { get; set; }
    }
}
