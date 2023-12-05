using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CallDetailRecordAPI.Structure.Models
{
    /// <summary>Represents a call details record (CDR).</summary>
    public class CallDetailRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        /// <summary>Gets or sets the unique reference for the call.</summary>
        public string Reference { get; set; }

        [BsonRequired]
        /// <summary>Gets or sets the caller phone number.</summary>
        public string CallerId { get; set; }

        [BsonRequired]
        /// <summary>Gets or sets the recipient phone number.</summary>
        public string Recipient { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        /// <summary>Gets or sets the call date.</summary>
        public DateTime CallDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        /// <summary>Gets or sets the time when the call ended.</summary>
        public DateTime EndTime { get; set; }

        /// <summary>Gets or sets the currency.</summary>
        public string Currency { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        public int Duration { get; set; }

        [BsonRequired]
        /// <summary>Gets or sets the call cost.</summary>
        public decimal Cost { get; set; }

        /// <summary>Gets or sets the call type.</summary>
        public CallType Type { get; set; }
    }
}
