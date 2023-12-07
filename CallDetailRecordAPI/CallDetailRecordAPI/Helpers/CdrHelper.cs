using CallDetailRecordAPI.Structure;
using CallDetailRecordAPI.Structure.Models;
using MongoDB.Driver;

namespace CallDetailRecordAPI.Helpers
{
    /// <summary>The CDR helper.</summary>
    public static partial class CdrHelper
    {
        /// <summary>Creates the query filters.</summary>
        /// <param name="callerId">The caller identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="type">The call type.</param>
        /// <returns>The query filters.</returns>
        public static FilterDefinition<CallDetailRecord> CreateQueryFilters(
            string callerId,
            DateTime startDate,
            DateTime endDate,
            CallType? type)
        {
            var filterBuilder = Builders<CallDetailRecord>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(item => item.CallerId, callerId),
                filterBuilder.Gte(item => item.CallDate, startDate),
                filterBuilder.Lt(item => item.CallDate, endDate)
            );

            if (type.HasValue && Enum.IsDefined(typeof(CallType), type.Value))
            {
                filter &= filterBuilder.Eq(item => item.Type, type.Value);
            }

            return filter;
        }

        /// <summary>Creates the call statistics query filters.</summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="type">The call type.</param>
        /// <returns>The query filters.</returns>
        public static FilterDefinition<CallDetailRecord> CreateCallStatisticsQueryFilter(
            DateTime startDate,
            DateTime endDate,
            CallType? type)
        {
            var filterBuilder = Builders<CallDetailRecord>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Gte(item => item.CallDate, startDate),
                filterBuilder.Lt(item => item.CallDate, endDate)
            );

            if (type.HasValue && Enum.IsDefined(typeof(CallType), type.Value))
            {
                filter &= filterBuilder.Eq(item => item.Type, type.Value);
            }

            return filter;
        }

        /// <summary>Determines if a call is domestic or international.</summary>
        /// <param name="callerNumber">The caller phone number.</param>
        /// <param name="recipientNumber">The recipient phone number.</param>
        /// <returns>The call type.</returns>
        public static CallType DetermineCallType(string callerNumber, string recipientNumber)
        {
            const string UkCountryCode = "44";

            if (recipientNumber.StartsWith(UkCountryCode) && callerNumber.StartsWith(UkCountryCode))
            {
                return CallType.Domestic; // UK (Domestic)
            }

            return CallType.International; // International
        }
    }
}
