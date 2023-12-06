using CallDetailRecordAPI.Structure.Converters;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace CallDetailRecordAPI.Structure.Models
{
    /// <summary>Represents the CSV call detail record (CDR).</summary>
    public class CsvCallDetailRecord
    {
        /// <summary>Gets or sets the caller phone number.</summary>
        public string caller_id { get; set; }

        /// <summary>Gets or sets the recipient phone number.</summary>
        public string recipient { get; set; }

        /// <summary>Gets or sets the call date.</summary>
        [TypeConverter(typeof(CustomDateTimeConverter))]
        public DateTime call_date { get; set; }

        /// <summary>Gets or sets the time when the call ended.</summary>
        public TimeSpan end_time { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        public int duration { get; set; }

        /// <summary>Gets or sets the call cost.</summary>
        public decimal cost { get; set; }

        /// <summary>Gets or sets the unique reference for the call.</summary>
        public string reference { get; set; }

        /// <summary>Gets or sets the currency.</summary>
        public string currency { get; set; }
    }
}
