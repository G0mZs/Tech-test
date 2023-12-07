using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace CallDetailRecordAPI.Structure.Requests
{
    /// <summary>Represents the call statistics request.</summary>
    public class CallStatisticsRequest
    {
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
