using CallDetailRecordAPI.Structure;
using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;
using MongoDB.Driver;

namespace CallDetailRecordAPI.Helpers
{
    /// <summary>The CDR helper.</summary>
    public static class CdrHelper
    {
        /// <summary>Creates the query filters.</summary>
        /// <param name="request">The request.</param>
        public static FilterDefinition<CallDetailRecord> CreateQueryFilters(CdrsRequest request)
        {
            var filterBuilder = Builders<CallDetailRecord>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(item => item.CallerId, request.CallerId),
                filterBuilder.Gte(item => item.CallDate, request.StartDate),
                filterBuilder.Lt(item => item.CallDate, request.EndDate)
            );

            if (request.Type.HasValue && Enum.IsDefined(typeof(CallType), request.Type.Value))
            {
                filter &= filterBuilder.Eq(item => item.Type, request.Type.Value);
            }

            return filter;
        }
    }
}
