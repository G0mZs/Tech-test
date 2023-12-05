using CallDetailRecordAPI.Structure;
using CallDetailRecordAPI.Structure.Models;

namespace CallDetailRecordAPI.Extensions
{
    /// <summary>Represents the CDR extensions.</summary>
    public static class CdrExtensions
    {
        /// <summary>Adds the services.</summary>
        /// <param name="csvCallDetailRecord">The csv CDR</param>
        /// <returns>The call detail record.</returns>
        public static CallDetailRecord ToCallDetailRecord(this CsvCallDetailRecord csvCallDetailRecord)
        {
            if (csvCallDetailRecord == null)
            {
                return null!;
            }

            return new CallDetailRecord
            {
                Reference = csvCallDetailRecord.reference,
                CallerId = csvCallDetailRecord.caller_id,
                Recipient = csvCallDetailRecord.recipient,
                CallDate = csvCallDetailRecord.call_date,
                EndTime = csvCallDetailRecord.end_time,
                Currency = csvCallDetailRecord.currency,
                Duration = csvCallDetailRecord.duration,
                Cost = csvCallDetailRecord.cost,
                Type = CallType.International
            };
        }
    }
}
