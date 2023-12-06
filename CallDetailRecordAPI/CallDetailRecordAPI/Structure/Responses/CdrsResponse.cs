using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;

namespace CallDetailRecordAPI.Structure.Responses
{
    /// <summary>Represents the call detail records response.</summary>
    public class CdrsResponse
    {
        /// <summary>The request data.</summary>
        public required CdrsRequest RequestData { get; set; }

        /// <summary>The data.</summary>
        public required IEnumerable<CallDetailRecord> Data { get; set; }
    }
}
