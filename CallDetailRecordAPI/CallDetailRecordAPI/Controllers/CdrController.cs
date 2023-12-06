using CallDetailRecordAPI.Services.Interfaces;
using CallDetailRecordAPI.Structure.Requests;
using CallDetailRecordAPI.Structure.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CallDetailRecordAPI.Controllers
{
    /// <summary>The call detail record controller.</summary>
    /// <seealso cref="ControllerBase"/>
    [Route("api/[controller]")]
    [ApiController]
    public class CdrController : ControllerBase
    {
        /// <summary>The call detail record service.</summary>
        private readonly ICdrService _cdrService;

        /// <param name="cdrService">The call detail record service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cdrService"/></exception>
        public CdrController(ICdrService cdrService)
        {
            _cdrService = cdrService ?? throw new ArgumentNullException(nameof(cdrService));
        }

        /// <summary>Uploads the call detail records CSV file.</summary>
        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadCallDetailRecordsCsvAsync(IFormFile file)
        {
            var result = await _cdrService.UploadCallDetailRecordsCsvAsync(file);

            if (!result)
            {
                return BadRequest("The request failed");
            }

            return Ok("The file was uploaded correctly !");
        }

        /// <summary>Gets the call detail record by his unique reference.</summary>
        [HttpGet("{reference}")]
        [ProducesResponseType(typeof(CdrResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCdrByReferenceAsync([FromRoute] string reference)
        {
            var data = await _cdrService.GetCdrByReferenceAsync(reference);

            return Ok(new CdrResponse
            {
                RequestData = new CdrRequest
                {
                    Reference = reference
                },
                Data = data
            });
        }

        /// <summary>Gets the total count and duration of calls in a specified time period.</summary>
        [HttpGet("Estatistics")]
        [ProducesResponseType(typeof(CallStatisticsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCallStatisticsAsync(
            [FromQuery] CallStatisticsRequest request)
        {
            var data = await _cdrService.GetCallStatisticsAsync(request);

            return Ok(new CallStatisticsResponse
            {
                RequestData = new CallStatisticsRequest
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type,
                },
                Data = data
            });
        }

        /// <summary>Gets the call detail records by caller identifier (phone number).</summary>
        [HttpGet("ByCallerId")]
        [ProducesResponseType(typeof(CdrsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCdrsByCallerIdAsync([FromQuery] CdrsRequest request)
        {
            var data = await _cdrService.GetCdrsByCallerIdAsync(request);

            return Ok(new CdrsResponse
            {
                RequestData = new CdrsRequest
                {
                    CallerId = request.CallerId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type
                },
                Data = data
            });
        }

        /// <summary>Gets the N most expensive calls by caller identifier, within a time period.</summary>
        [HttpGet("MostExpensiveCalls")]
        [ProducesResponseType(typeof(MostExpensiveCallsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMostExpensiveCallsByCallerIdAsync(
            [FromQuery] MostExpensiveCallsRequest request)
        {
            var data = await _cdrService.GetMostExpensiveCallsByCallerIdAsync(request);

            return Ok(new MostExpensiveCallsResponse
            {
                RequestData = new MostExpensiveCallsRequest
                {
                    CallerId = request.CallerId,
                    Take = request.Take,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type,
                },
                Data = data
            });
        }
    }
}
