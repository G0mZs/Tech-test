using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CallDetailRecordAPI.Structure.Models
{
    /// <summary>Represents a call detail record (CDR).</summary>
    public class CallDetailRecord
    {
        /// <summary>Gets or sets the unique reference for the call.</summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Reference { get; set; }

        /// <summary>Gets or sets the caller phone number.</summary>
        [BsonRequired]
        public required string CallerId { get; set; }

        /// <summary>Gets or sets the recipient phone number.</summary>
        [BsonRequired]
        public required string Recipient { get; set; }

        /// <summary>Gets or sets the call date.</summary>
        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public required DateTime CallDate { get; set; }

        /// <summary>Gets or sets the time when the call ended.</summary>
        [BsonRequired]
        public required TimeSpan EndTime { get; set; }

        /// <summary>Gets or sets the currency.</summary>
        public required string Currency { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        public int Duration { get; set; }

        /// <summary>Gets or sets the call cost.</summary>
        [BsonRequired]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Cost { get; set; }

        /// <summary>Gets or sets the call type.</summary>
        [BsonRequired]
        public CallType Type { get; set; }
    }
}
