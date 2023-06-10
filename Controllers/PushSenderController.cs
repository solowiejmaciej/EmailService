using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class PushSenderController : ControllerBase
    {
        private readonly IPushSenderService _pushSenderService;

        public PushSenderController(IPushSenderService pushSenderService)
        {
            _pushSenderService = pushSenderService;
        }

        [Route("SendPushNow")]
        [HttpPost]
        public IActionResult SendPushNow([FromBody] PushRequest pushRequest, [FromQuery] string userId)
        {
            _pushSenderService.SendPushNow(pushRequest, userId);
            return Ok(pushRequest);
        }
    }
}