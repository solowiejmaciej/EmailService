using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailSenderController : ControllerBase
    {
        private readonly ILogger<EmailSenderController> _logger;
        private readonly IEmailSenderService _emailSenderService;

        public EmailSenderController(ILogger<EmailSenderController> logger, IEmailSenderService emailSenderService)
        {
            _logger = logger;
            _emailSenderService = emailSenderService;
        }

        [Route("AddNewEmailToDb")]
        [HttpPost]
        public async Task<IActionResult> AddNewEmailToDb(EmailDto email)
        {
            await _emailSenderService.AddNewEmailToDbAsync(email);
            return Ok(email);
        }

        [Route("SendEmailNow")]
        [HttpPost]
        public async Task<IActionResult> SendEmailNow(EmailDto email)
        {
            await _emailSenderService.SendEmailNow(email);
            return Ok(email);
        }
    }
}