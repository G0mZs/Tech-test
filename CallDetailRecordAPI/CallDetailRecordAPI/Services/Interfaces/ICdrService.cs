using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;

namespace CallDetailRecordAPI.Services.Interfaces
{
    ///<summary>Represents the CDR service interface.</summary>
    public interface ICdrService
    {
        /// <summary>Uploads the call detail records CSV file asynchronous.</summary>
        /// <param name="file">The file.</param>
        /// <return>True if sucessful, otherwise false.</return>
        Task<bool> UploadCallDetailRecordsCsvAsync(IFormFile file);

        /// <summary>Gets the CDR by reference asynchronous.</summary>
        /// <param name="reference">The CDR unique reference.</param>
        /// <return>The call detail record.</return>
        Task<CallDetailRecord> GetCdrByReferenceAsync(string reference);

        /// <summary>Gets the CDRs by caller identifier asynchronous.</summary>
        /// <param name="request">The CDRs request.</param>
        /// <return>An enumerable of call detail records.</return>
        Task<IEnumerable<CallDetailRecord>> GetCdrsByCallerIdAsync(CdrsRequest request);
    }
}
