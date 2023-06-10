using System.Security.Claims;
using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EmailSenderController : ControllerBase
    {
        private readonly IEmailSenderService _emailSenderService;

        public EmailSenderController(IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
        }

        [Route("SendEmailNow")]
        [HttpPost]
        public async Task<IActionResult> SendEmailNow(EmailRequest email)
        {
            await _emailSenderService.SendEmailNow(email);
            return Ok(email);
        }
    }
}