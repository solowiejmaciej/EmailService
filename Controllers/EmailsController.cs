using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Entities;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;
using NotificationService.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotificationService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailDataService _emailDataService;

        public EmailsController(IEmailDataService emailDataService)
        {
            _emailDataService = emailDataService;
        }

        [HttpGet]
        public ActionResult<List<EmailDto>> GetAll()
        {
            var allEmails = _emailDataService.GetAllEmails();
            return Ok(allEmails);
        }

        [HttpGet("{id}")]
        public ActionResult<EmailDto> Get(int id)
        {
            return Ok(_emailDataService.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmailRequest email)
        {
            await _emailDataService.AddNewEmailToDbAsync(email);
            return Ok(email);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _emailDataService.SoftDelete(id);
            return Ok();
        }
    }
}