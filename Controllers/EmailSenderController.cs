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