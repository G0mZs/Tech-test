using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Services;
using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CallDetailRecordAPI.Tests.UnitTests.Services
{
    public class CdrServiceTests
    {
        private readonly Mock<IMongoDatabase> _mockMongoDatabase;
        private readonly Mock<IOptions<CdrDatabaseConfiguration>> _mockOptions;
        private readonly Mock<IMongoCollection<CallDetailRecord>> _mockCdrCollection;
        private readonly Mock<IAsyncCursor<CallDetailRecord>> _mockAsyncCursor;

        private readonly CdrService _service;

        private readonly Fixture _fixture;

        public CdrServiceTests()
        {
            _fixture = new Fixture();

            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockOptions = new Mock<IOptions<CdrDatabaseConfiguration>>();
            _mockCdrCollection = new Mock<IMongoCollection<CallDetailRecord>>();
            _mockAsyncCursor = new Mock<IAsyncCursor<CallDetailRecord>>();

            _mockMongoDatabase.Setup(item => item.GetCollection<CallDetailRecord>(It.IsAny<string>(), null))
                .Returns(_mockCdrCollection.Object);

            _mockOptions.Setup(item => item.Value)
                .Returns(_fixture.Create<CdrDatabaseConfiguration>);

            _service = new CdrService(
                _mockMongoDatabase.Object,
                _mockOptions.Object);
        }

        #region Constructor

        [Fact]
        public void Should_ThrowArgumentNullException_When_DatabaseIsNull()
        {
            IMongoDatabase database = null!;

            FluentActions.Invoking(() => new CdrService(database, _mockOptions.Object))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName(nameof(database));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_OptionsAreNull()
        {
            IOptions<CdrDatabaseConfiguration> options = null!;

            FluentActions.Invoking(() => new CdrService(_mockMongoDatabase.Object, options))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName(nameof(options));
        }

        #endregion

        #region UploadCallDetailRecordsCsvAsync

        [Fact]
        public async Task UploadCallDetailRecordsCsvAsync_NullFile_ThrowsArgumentException()
        {
            IFormFile file = null!;

            await FluentActions.Invoking(() => _service.UploadCallDetailRecordsCsvAsync(file))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(file));
        }

        [Fact]
        public async Task UploadCallDetailRecordsCsvAsync_EmptyFile_ThrowsArgumentException()
        {
            var file = new Mock<IFormFile>();

            file.Setup(f => f.Length)
                .Returns(0);

            await FluentActions.Invoking(() => _service.UploadCallDetailRecordsCsvAsync(file.Object))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(file));
        }

        [Fact]
        public async Task UploadCallDetailRecordsCsvAsync_ValidFile_ReturnTrue()
        {
            var csvContent = "caller_id,recipient,call_date,end_time,duration,cost,reference,currency";
            var csvFile = CreateCsvFileFromString(csvContent);

            var result = await _service.UploadCallDetailRecordsCsvAsync(csvFile);

            result.Should().BeTrue();

            _mockCdrCollection.Verify(item => item.InsertManyAsync(
                    It.IsAny<IEnumerable<CallDetailRecord>>(),
                    It.IsAny<InsertManyOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region GetCdrByReferenceAsync

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetCdrByReferenceAsync_InvalidReference_ThrowsArgumentException(string reference)
        {
            await FluentActions.Invoking(() => _service.GetCdrByReferenceAsync(reference))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(reference));
        }

        [Fact]
        public async Task GetCdrByReferenceAsync_ValidReference_ReturnsCdr()
        {
            const string Reference = "test";

            _mockCdrCollection.Setup(item => item.FindAsync(
                It.IsAny<FilterDefinition<CallDetailRecord>>(),
                It.IsAny<FindOptions<CallDetailRecord, CallDetailRecord>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockAsyncCursor.Object);

            var result = await _service.GetCdrByReferenceAsync(Reference);

            _mockCdrCollection.Verify(item => item.FindAsync(
                    It.IsAny<FilterDefinition<CallDetailRecord>>(),
                    It.IsAny<FindOptions<CallDetailRecord, CallDetailRecord>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region GetCallStatisticsAsync

        [Fact]
        public async Task GetCallStatisticsAsync_NullRequest_ThrowsArgumentNullException()
        {
            CallStatisticsRequest request = null!;

            await FluentActions.Invoking(() => _service.GetCallStatisticsAsync(request))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName(nameof(request));
        }

        [Fact]
        public async Task GetCallStatisticsAsync_StartDateInTheFuture_ThrowsArgumentException()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(15);

            var request = _fixture.Build<CallStatisticsRequest>()
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, DateTime.UtcNow)
                .Create();

            await FluentActions.Invoking(() => _service.GetCallStatisticsAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(startDate));
        }

        [Fact]
        public async Task GetCallStatisticsAsync_EndDateInTheFuture_ThrowsArgumentException()
        {
            DateTime endDate = DateTime.UtcNow.AddDays(15);

            var request = _fixture.Build<CallStatisticsRequest>()
                .With(item => item.StartDate, DateTime.UtcNow.AddDays(-15))
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetCallStatisticsAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetCallStatisticsAsync_StartDateGreaterThanEndDate_ThrowsArgumentException()
        {
            DateTime endDate = DateTime.UtcNow.AddDays(-15);
            DateTime startDate = DateTime.UtcNow.AddDays(-14);

            var request = _fixture.Build<CallStatisticsRequest>()
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetCallStatisticsAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetCallStatisticsAsync_TimeRangeExceedsOneMonth_ThrowsArgumentException()
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddMonths(-2);

            var request = _fixture.Build<CallStatisticsRequest>()
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetCallStatisticsAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetCallStatisticsAsync_ValidRequest_ReturnsCallEstatistics()
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-15);

            var request = _fixture.Build<CallStatisticsRequest>()
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            var mockAsyncCursor = new Mock<IAsyncCursor<BsonDocument>>();

            _mockCdrCollection.Setup(item => item.AggregateAsync(
                It.IsAny<PipelineDefinition<CallDetailRecord, BsonDocument>>(),
                It.IsAny<AggregateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAsyncCursor.Object);

            var result = await _service.GetCallStatisticsAsync(request);

            result.Should().NotBeNull()
                .And.BeOfType<CallStatistics>();

            _mockCdrCollection.Verify(item => item.AggregateAsync(
                    It.IsAny<PipelineDefinition<CallDetailRecord, BsonDocument>>(),
                    It.IsAny<AggregateOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region GetCdrsByCallerIdAsync

        [Fact]
        public async Task GetCdrsByCallerIdAsync_NullRequest_ThrowsArgumentNullException()
        {
            CdrsRequest request = null!;

            await FluentActions.Invoking(() => _service.GetCdrsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName(nameof(request));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetCdrsByCallerIdAsync_InvalidCallerId_ThrowsArgumentException(string callerId)
        {
            var request = _fixture.Build<CdrsRequest>()
                .With(item => item.CallerId, callerId)
                .With(item => item.StartDate, DateTime.UtcNow.AddDays(-15))
                .With(item => item.EndDate, DateTime.UtcNow)
                .Create();

            await FluentActions.Invoking(() => _service.GetCdrsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(callerId));
        }

        [Fact]
        public async Task GetCdrsByCallerIdAsync_StartDateInTheFuture_ThrowsArgumentException()
        {
            const string CallerId = "test";
            DateTime startDate = DateTime.UtcNow.AddDays(15);

            var request = _fixture.Build<CdrsRequest>()
                .With(item => item.CallerId, CallerId)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, DateTime.UtcNow)
                .Create();

            await FluentActions.Invoking(() => _service.GetCdrsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(startDate));
        }

        [Fact]
        public async Task GetCdrsByCallerIdAsync_EndDateInTheFuture_ThrowsArgumentException()
        {
            const string CallerId = "test";
            DateTime endDate = DateTime.UtcNow.AddDays(15);

            var request = _fixture.Build<CdrsRequest>()
                .With(item => item.CallerId, CallerId)
                .With(item => item.StartDate, DateTime.UtcNow.AddDays(-15))
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetCdrsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetCdrsByCallerIdAsync_StartDateGreaterThanEndDate_ThrowsArgumentException()
        {
            const string CallerId = "test";
            DateTime endDate = DateTime.UtcNow.AddDays(-15);
            DateTime startDate = DateTime.UtcNow.AddDays(-14);

            var request = _fixture.Build<CdrsRequest>()
                .With(item => item.CallerId, CallerId)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetCdrsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetCdrsByCallerIdAsync_TimeRangeExceedsOneMonth_ThrowsArgumentException()
        {
            const string CallerId = "test";
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddMonths(-2);

            var request = _fixture.Build<CdrsRequest>()
                .With(item => item.CallerId, CallerId)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetCdrsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetCdrsByCallerIdAsync_ValidRequest_ReturnsCdrs()
        {
            const string CallerId = "test";
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-15);

            var request = _fixture.Build<CdrsRequest>()
                .With(item => item.CallerId, CallerId)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            _mockCdrCollection.Setup(item => item.FindAsync(
                It.IsAny<FilterDefinition<CallDetailRecord>>(),
                It.IsAny<FindOptions<CallDetailRecord, CallDetailRecord>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockAsyncCursor.Object);

            var result = await _service.GetCdrsByCallerIdAsync(request);

            result.Should().NotBeNull()
                .And.AllBeOfType<CallDetailRecord>();

            _mockCdrCollection.Verify(item => item.FindAsync(
                    It.IsAny<FilterDefinition<CallDetailRecord>>(),
                    It.IsAny<FindOptions<CallDetailRecord, CallDetailRecord>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region GetMostExpensiveCallsByCallerIdAsync

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_NullRequest_ThrowsArgumentNullException()
        {
            MostExpensiveCallsRequest request = null!;

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName(nameof(request));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetMostExpensiveCallsByCallerIdAsync_InvalidCallerId_ThrowsArgumentException(
            string callerId)
        {
            const int Take = 1;

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.CallerId, callerId)
                .With(item => item.Take, Take)
                .With(item => item.StartDate, DateTime.UtcNow.AddDays(-15))
                .With(item => item.EndDate, DateTime.UtcNow)
                .Create();

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(callerId));
        }

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_InvalidTake_ThrowsArgumentException()
        {
            var take = 0;

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.Take, take)
                .With(item => item.StartDate, DateTime.UtcNow.AddDays(-15))
                .With(item => item.EndDate, DateTime.UtcNow)
                .Create();

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(take));
        }

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_StartDateInTheFuture_ThrowsArgumentException()
        {
            const int Take = 1;
            DateTime startDate = DateTime.UtcNow.AddDays(15);

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.Take, Take)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, DateTime.UtcNow)
                .Create();

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(startDate));
        }

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_EndDateInTheFuture_ThrowsArgumentException()
        {
            const int Take = 1;
            DateTime endDate = DateTime.UtcNow.AddDays(15);

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.Take, Take)
                .With(item => item.StartDate, DateTime.UtcNow.AddDays(-15))
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_StartDateGreaterThanEndDate_ThrowsArgumentException()
        {
            const int Take = 1;
            DateTime endDate = DateTime.UtcNow.AddDays(-15);
            DateTime startDate = DateTime.UtcNow.AddDays(-14);

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.Take, Take)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_TimeRangeExceedsOneMonth_ThrowsArgumentException()
        {
            const int Take = 1;
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddMonths(-2);

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.Take, Take)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            await FluentActions.Invoking(() => _service.GetMostExpensiveCallsByCallerIdAsync(request))
                .Should().ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(endDate));
        }

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_ValidRequest_ReturnsMostExpensiveCalls()
        {
            const int Take = 1;
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-15);

            var request = _fixture.Build<MostExpensiveCallsRequest>()
                .With(item => item.Take, Take)
                .With(item => item.StartDate, startDate)
                .With(item => item.EndDate, endDate)
                .Create();

            _mockCdrCollection.Setup(item => item.FindAsync(
                It.IsAny<FilterDefinition<CallDetailRecord>>(),
                It.IsAny<FindOptions<CallDetailRecord, CallDetailRecord>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockAsyncCursor.Object);

            var result = await _service.GetMostExpensiveCallsByCallerIdAsync(request);

            result.Should().NotBeNull()
                .And.AllBeOfType<CallDetailRecord>();

            _mockCdrCollection.Verify(item => item.FindAsync(
                    It.IsAny<FilterDefinition<CallDetailRecord>>(),
                    It.IsAny<FindOptions<CallDetailRecord, CallDetailRecord>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Private Methods

        private IFormFile CreateCsvFileFromString(string csvContent)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(csvContent);
            writer.Flush();
            stream.Position = 0;

            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("test.csv");
            file.Setup(f => f.Length).Returns(stream.Length);
            file.Setup(f => f.OpenReadStream()).Returns(stream);

            return file.Object;
        }

        #endregion
    }
}
