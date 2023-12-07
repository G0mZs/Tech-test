using CallDetailRecordAPI.Helpers;
using CallDetailRecordAPI.Structure.Models;

namespace CallDetailRecordAPI.Extensions
{
    /// <summary>Represents the CDR extensions.</summary>
    public static class CdrExtensions
    {
        #region Public Methods

        /// <summary>Converts a <see cref="CsvCallDetailRecord"/>into a <see cref="CallDetailRecord"/>.</summary>
        /// <param name="csvCallDetailRecord">The csv CDR</param>
        /// <returns>The call detail record.</returns>
        public static CallDetailRecord ToCallDetailRecord(this CsvCallDetailRecord csvCallDetailRecord)
        {
            if (csvCallDetailRecord == null)
            {
                return null!;
            }

            if (string.IsNullOrWhiteSpace(csvCallDetailRecord.caller_id) ||
                string.IsNullOrWhiteSpace(csvCallDetailRecord.recipient))
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
                Type = CdrHelper.DetermineCallType(csvCallDetailRecord.caller_id, csvCallDetailRecord.recipient)
            };
        }

        #endregion
    }
}
