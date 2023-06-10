using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PushController : ControllerBase
    {
        private readonly IPushDataService _pushDataService;

        public PushController(IPushDataService pushDataService)
        {
            _pushDataService = pushDataService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PushRequest pushRequest, [FromQuery] string userId)
        {
            await _pushDataService.AddNewAsync(pushRequest, userId);
            return Ok(pushRequest);
        }
    }
}