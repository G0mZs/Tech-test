using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace CallDetailRecordAPI.Structure.Requests
{
    /// <summary>Represents the most expensive calls request.</summary>
    public class MostExpensiveCallsRequest
    {
        ///<summary>The caller identifier (phone number).</summary>
        [BindRequired]
        public required string CallerId { get; set; }

        ///<summary>The number of calls to be shown.</summary>
        [BindRequired]
        public required int Take { get; set; }

        ///<summary>The start date.</summary>
        [BindRequired]
        public required DateTime StartDate { get; set; }

        ///<summary>The end date.</summary>
        [BindRequired]
        public required DateTime EndDate { get; set; }

        ///<summary>The call type.</summary>
        [DefaultValue(null)]
        public CallType? Type { get; set; }
    }
}
