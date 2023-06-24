using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SenderController : ControllerBase
    {
        private readonly IPushSenderService _pushSenderService;
        private readonly IEmailSenderService _emailSenderService;

        public SenderController(IPushSenderService pushSenderService, IEmailSenderService emailSenderService)
        {
            _pushSenderService = pushSenderService;
            _emailSenderService = emailSenderService;
        }

        [HttpPost("Push")]
        public IActionResult SendPushNow([FromBody] PushRequest pushRequest, [FromQuery] string userId)
        {
            _pushSenderService.SendPushNow(pushRequest, userId);
            return Ok(pushRequest);
        }

        [HttpPost("Email")]
        public async Task<IActionResult> SendEmailNow(EmailRequest email)
        {
            await _emailSenderService.SendEmailNow(email);
            return Ok(email);
        }

        [HttpPost("Sms")]
        public async Task<IActionResult> SendSmsNow()
        {
            return Ok(1);
        }
    }
}