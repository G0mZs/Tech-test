using CallDetailRecordAPI.Extensions;
using CallDetailRecordAPI.Structure;
using CallDetailRecordAPI.Structure.Models;

namespace CallDetailRecordAPI.Tests.UnitTests.Extensions
{
    public class CdrExtensionsTests
    {
        private readonly Fixture _fixture = new();

        #region ToCallDetailRecord

        [Fact]
        public void ToCallDetailRecord_NullCsvCallDetailRecord_ReturnsNull()
        {
            CsvCallDetailRecord csvCallDetailRecord = null!;

            var result = csvCallDetailRecord.ToCallDetailRecord();

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ToCallDetailRecord_InvalidCallerId_ReturnsNull(string callerId)
        {
            var csvCallDetailRecord = _fixture.Build<CsvCallDetailRecord>()
                .With(item => item.caller_id, callerId)
                .Create();

            var result = csvCallDetailRecord.ToCallDetailRecord();

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ToCallDetailRecord_InvalidRecipient_ReturnsNull(string recipient)
        {
            var csvCallDetailRecord = _fixture.Build<CsvCallDetailRecord>()
                .With(item => item.recipient, recipient)
                .Create();

            var result = csvCallDetailRecord.ToCallDetailRecord();

            result.Should().BeNull();
        }

        [Fact]
        public void ToCallDetailRecord_WithCorrectData_ReturnsCallDetailRecord()
        {
            var csvCallDetailRecord = _fixture.Create<CsvCallDetailRecord>();

            var result = csvCallDetailRecord.ToCallDetailRecord();

            result.Should().NotBeNull()
                .And.BeOfType<CallDetailRecord>();

            result.Reference.Should().Be(csvCallDetailRecord.reference);
            result.CallerId.Should().Be(csvCallDetailRecord.caller_id);
            result.Recipient.Should().Be(csvCallDetailRecord.recipient);
            result.CallDate.Should().Be(csvCallDetailRecord.call_date);
            result.EndTime.Should().Be(csvCallDetailRecord.end_time);
            result.Currency.Should().Be(csvCallDetailRecord.currency);
            result.Duration.Should().Be(csvCallDetailRecord.duration);
            result.Cost.Should().Be(csvCallDetailRecord.cost);
            result.Type.Should().Be(CallType.International);
        }

        #endregion
    }
}
