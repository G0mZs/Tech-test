using CallDetailRecordAPI.Helpers;
using CallDetailRecordAPI.Structure;
using CallDetailRecordAPI.Structure.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CallDetailRecordAPI.Tests.UnitTests.Helpers
{
    public class CdrHelperTests
    {
        private readonly Fixture _fixture = new();

        #region CreateQueryFilters

        [Fact]
        public void CreateQueryFilters_WithCorrectData_ReturnsQueryFilter()
        {
            var callerId = _fixture.Create<string>();
            var startDate = _fixture.Create<DateTime>();
            var endDate = startDate.AddDays(1);
            var type = _fixture.Create<CallType?>();

            var result = CdrHelper.CreateQueryFilters(callerId, startDate, endDate, type);

            result.Should().NotBeNull()
                .And.BeAssignableTo<FilterDefinition<CallDetailRecord>>();

            var filter = result.As<FilterDefinition<CallDetailRecord>>();

            var filterBuilder = Builders<CallDetailRecord>.Filter;
            var expectedFilter = filterBuilder.And(
                filterBuilder.Eq(item => item.CallerId, callerId),
                filterBuilder.Gte(item => item.CallDate, startDate),
                filterBuilder.Lt(item => item.CallDate, endDate),
                filterBuilder.Eq(item => item.Type, type)
            );

            var expectedJson = ConvertFilterToJson(expectedFilter);

            var resultJson = ConvertFilterToJson(result);

            resultJson.Should().Be(expectedJson);
        }

        #endregion

        #region CreateCallStatisticsQueryFilter

        [Fact]
        public void CreateCallEstatisticsQueryFilter_WithCorrectData_ReturnsQueryFilter()
        {
            var startDate = _fixture.Create<DateTime>();
            var endDate = startDate.AddDays(1);
            var type = _fixture.Create<CallType?>();

            var result = CdrHelper.CreateCallStatisticsQueryFilter(startDate, endDate, type);

            result.Should().NotBeNull()
                .And.BeAssignableTo<FilterDefinition<CallDetailRecord>>();

            var filter = result.As<FilterDefinition<CallDetailRecord>>();

            var filterBuilder = Builders<CallDetailRecord>.Filter;
            var expectedFilter = filterBuilder.And(
                filterBuilder.Gte(item => item.CallDate, startDate),
                filterBuilder.Lt(item => item.CallDate, endDate),
                filterBuilder.Eq(item => item.Type, type)
            );

            var expectedJson = ConvertFilterToJson(expectedFilter);

            var resultJson = ConvertFilterToJson(result);

            resultJson.Should().Be(expectedJson);
        }

        #endregion

        #region DetermineCallType

        [Fact]
        public void DetermineCallType_WithUkPhoneNumbers_ReturnsDomesticCallType()
        {
            string callerNumberUK = "441234567890";
            string recipientNumberUK = "441987654321";

            var result = CdrHelper.DetermineCallType(callerNumberUK, recipientNumberUK);

            result.Should().Be(CallType.Domestic);
        }

        [Fact]
        public void DetermineCallType_WithInternationalPhoneNumbers_ReturnsInternationalCallType()
        {
            string callerNumberInternational = "491234567890";
            string recipientNumberInternational = "499876543210";

            var result = CdrHelper.DetermineCallType(callerNumberInternational, recipientNumberInternational);

            result.Should().Be(CallType.International);
        }

        [Fact]
        public void DetermineCallType_WithUkCallerIdAndInternationalRecipient_ReturnsInternationalCallType()
        {
            string callerNumberInternational = "441234567890";
            string recipientNumberInternational = "499876543210";

            var result = CdrHelper.DetermineCallType(callerNumberInternational, recipientNumberInternational);

            result.Should().Be(CallType.International);
        }

        [Fact]
        public void DetermineCallType_WithInternationalCallerIdAndUkRecipient_ReturnsInternationalCallType()
        {
            string callerNumberInternational = "491234567890";
            string recipientNumberInternational = "441234567890";

            var result = CdrHelper.DetermineCallType(callerNumberInternational, recipientNumberInternational);

            result.Should().Be(CallType.International);
        }

        #endregion

        #region Private Methods

        private static string ConvertFilterToJson(FilterDefinition<CallDetailRecord> filter)
        {
            var serializerRegistry = MongoDB.Bson.Serialization.BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<CallDetailRecord>();

            return filter.Render(documentSerializer, serializerRegistry).ToJson();
        }

        #endregion
    }
}
