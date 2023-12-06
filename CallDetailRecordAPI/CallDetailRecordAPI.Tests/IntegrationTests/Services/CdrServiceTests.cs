using CallDetailRecordAPI.Structure.Models;

namespace CallDetailRecordAPI.Tests.IntegrationTests.Services
{
    public class CdrServiceTests : IClassFixture<MongoDatabaseFixture>
    {
        private readonly MongoDatabaseFixture _databaseFixture;
        private readonly Fixture _fixture = new();

        public CdrServiceTests(MongoDatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture ?? throw new ArgumentNullException(nameof(databaseFixture));
        }

        [Fact]
        public async Task GetCdrByReferenceAsync_ValidReference_ReturnsCdr()
        {
            const string Reference = "test";
            var expectedCallDetailRecord = _fixture.Build<CallDetailRecord>()
                .With(item => item.Reference, Reference)
                .Create();

            await _databaseFixture.CdrCollection.InsertOneAsync(expectedCallDetailRecord);

            var result = await _databaseFixture.Service.GetCdrByReferenceAsync(Reference);

            result.Should().NotBeNull()
                .And.BeOfType<CallDetailRecord>();

            result.Reference.Should().Be(expectedCallDetailRecord.Reference);
        }
    }
}
