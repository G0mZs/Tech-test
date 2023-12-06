using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;

namespace CallDetailRecordAPI.Structure.Responses
{
    /// <summary>Represents the call detail record response.</summary>
    public class CdrResponse
    {
        /// <summary>The request data.</summary>
        public required CdrRequest RequestData { get; set; }

        /// <summary>The data.</summary>
        public required CallDetailRecord Data { get; set; }
    }
}
