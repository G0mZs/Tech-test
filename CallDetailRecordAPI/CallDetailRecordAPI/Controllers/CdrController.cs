using CallDetailRecordAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CallDetailRecordAPI.Controllers
{
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
    }
}
