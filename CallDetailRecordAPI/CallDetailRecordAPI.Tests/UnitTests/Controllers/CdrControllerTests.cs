using CallDetailRecordAPI.Controllers;
using CallDetailRecordAPI.Services.Interfaces;
using CallDetailRecordAPI.Structure.Models;
using CallDetailRecordAPI.Structure.Requests;
using CallDetailRecordAPI.Structure.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CallDetailRecordAPI.Tests.UnitTests.Controllers
{
    public class CdrControllerTests
    {
        private readonly CdrController _controller;

        private readonly Mock<ICdrService> _mockCdrService;

        private readonly Fixture _fixture;

        public CdrControllerTests()
        {
            _mockCdrService = new Mock<ICdrService>();
            _controller = new CdrController(_mockCdrService.Object);
            _fixture = new Fixture();
        }

        #region Constructor

        [Fact]
        public void Should_ThrowArgumentNullException_When_CdrServiceIsNull()
        {
            ICdrService cdrService = null!;

            FluentActions.Invoking(() => new CdrController(cdrService))
                .Should()
                .Throw<ArgumentNullException>()
                .WithParameterName(nameof(cdrService));
        }

        #endregion

        #region UploadCallDetailRecordsCsvAsync

        [Fact]
        public async Task UploadCallDetailRecordsCsvAsync_SuccessfulUpload_ReturnsOk()
        {
            var fileBytes = _fixture.Create<byte[]>();
            var length = _fixture.Create<long>();
            var fileName = _fixture.Create<string>();
            var contentType = _fixture.Create<string>();

            var formFile = new FormFile(
                baseStream: new MemoryStream(fileBytes),
                baseStreamOffset: 0,
                length: length,
                name: "file",
                fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            _mockCdrService.Setup(s => s.UploadCallDetailRecordsCsvAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(true);

            var result = await _controller.UploadCallDetailRecordsCsvAsync(formFile);

            result.Should().NotBeNull()
                .And.BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            _mockCdrService.Verify(s => s.UploadCallDetailRecordsCsvAsync(
                    It.IsAny<IFormFile>()),
                Times.Once);

        }

        [Fact]
        public async Task UploadCallDetailRecordsCsvAsync_UnsuccessfulUpload_ReturnsBadRequest()
        {
            var fileBytes = _fixture.Create<byte[]>();
            var length = _fixture.Create<long>();
            var fileName = _fixture.Create<string>();
            var contentType = _fixture.Create<string>();

            var formFile = new FormFile(
                baseStream: new System.IO.MemoryStream(fileBytes),
                baseStreamOffset: 0,
                length: length,
                name: "file",
                fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            _mockCdrService.Setup(s => s.UploadCallDetailRecordsCsvAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(false);

            var result = await _controller.UploadCallDetailRecordsCsvAsync(formFile);

            result.Should().NotBeNull()
                .And.BeOfType<BadRequestObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _mockCdrService.Verify(s => s.UploadCallDetailRecordsCsvAsync(
                    It.IsAny<IFormFile>()),
                Times.Once);
        }

        #endregion

        #region GetCdrByReferenceAsync

        [Fact]
        public async Task GetCdrByReferenceAsync_WithCorrectData_ReturnsOk()
        {
            var reference = _fixture.Create<string>();
            var callDetailRecord = _fixture.Create<CallDetailRecord>();

            _mockCdrService.Setup(s => s.GetCdrByReferenceAsync(It.IsAny<string>()))
                .ReturnsAsync(callDetailRecord);

            var result = await _controller.GetCdrByReferenceAsync(reference);

            result.Should().NotBeNull()
                .And.BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResult = (OkObjectResult)result;

            var cdrResponse = okResult.Value.Should().NotBeNull()
                .And.BeOfType<CdrResponse>().Subject;

            cdrResponse.RequestData.Should().NotBeNull()
                .And.BeOfType<CdrRequest>();
            cdrResponse.Data.Should().NotBeNull()
                .And.BeOfType<CallDetailRecord>();

            _mockCdrService.Verify(s => s.GetCdrByReferenceAsync(
                    It.IsAny<string>()),
                Times.Once);
        }

        #endregion

        #region GetCallStatisticsAsync

        [Fact]
        public async Task GetCallStatisticsAsync_WithCorrectData_ReturnsOk()
        {
            var request = _fixture.Create<CallStatisticsRequest>();
            var callEstatistics = _fixture.Create<CallStatistics>();

            _mockCdrService.Setup(s => s.GetCallStatisticsAsync(It.IsAny<CallStatisticsRequest>()))
                .ReturnsAsync(callEstatistics);

            var result = await _controller.GetCallStatisticsAsync(request);

            result.Should().NotBeNull()
                .And.BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResult = (OkObjectResult)result;

            var callEstatisticsResponse = okResult.Value.Should().NotBeNull()
                .And.BeOfType<CallStatisticsResponse>().Subject;

            callEstatisticsResponse.RequestData.Should().NotBeNull()
                .And.BeOfType<CallStatisticsRequest>();
            callEstatisticsResponse.Data.Should().NotBeNull()
                .And.BeOfType<CallStatistics>();

            _mockCdrService.Verify(s => s.GetCallStatisticsAsync(
                    It.IsAny<CallStatisticsRequest>()),
                Times.Once);
        }

        #endregion

        #region GetCdrsByCallerIdAsync

        [Fact]
        public async Task GetCdrsByCallerIdAsync_WithCorrectData_ReturnsOk()
        {
            var request = _fixture.Create<CdrsRequest>();
            var callDetailRecords = _fixture.CreateMany<CallDetailRecord>(5);

            _mockCdrService.Setup(s => s.GetCdrsByCallerIdAsync(It.IsAny<CdrsRequest>()))
                .ReturnsAsync(callDetailRecords);

            var result = await _controller.GetCdrsByCallerIdAsync(request);

            result.Should().NotBeNull()
                .And.BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResult = (OkObjectResult)result;

            var cdrsResponse = okResult.Value.Should().NotBeNull()
                .And.BeOfType<CdrsResponse>().Subject;

            cdrsResponse.RequestData.Should().NotBeNull()
                .And.BeOfType<CdrsRequest>();
            cdrsResponse.Data.Should().NotBeNull()
                .And.AllBeOfType<CallDetailRecord>();

            _mockCdrService.Verify(s => s.GetCdrsByCallerIdAsync(
                    It.IsAny<CdrsRequest>()),
                Times.Once);
        }

        #endregion

        #region GetMostExpensiveCallsByCallerIdAsync

        [Fact]
        public async Task GetMostExpensiveCallsByCallerIdAsync_WithCorrectData_ReturnsOk()
        {
            var request = _fixture.Create<MostExpensiveCallsRequest>();
            var callDetailRecords = _fixture.CreateMany<CallDetailRecord>(5);

            _mockCdrService.Setup(s => s.GetMostExpensiveCallsByCallerIdAsync(
                    It.IsAny<MostExpensiveCallsRequest>()))
                .ReturnsAsync(callDetailRecords);

            var result = await _controller.GetMostExpensiveCallsByCallerIdAsync(request);

            result.Should().NotBeNull()
                .And.BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResult = (OkObjectResult)result;

            var mostExpensiveCallsResponse = okResult.Value.Should().NotBeNull()
                .And.BeOfType<MostExpensiveCallsResponse>().Subject;

            mostExpensiveCallsResponse.RequestData.Should().NotBeNull()
                .And.BeOfType<MostExpensiveCallsRequest>();
            mostExpensiveCallsResponse.Data.Should().NotBeNull()
                .And.AllBeOfType<CallDetailRecord>();

            _mockCdrService.Verify(s => s.GetMostExpensiveCallsByCallerIdAsync(
                    It.IsAny<MostExpensiveCallsRequest>()),
                Times.Once);
        }

        #endregion
    }
}
