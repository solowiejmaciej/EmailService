﻿using EmailService.Entities;
using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmailService.Controllers
{
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
        public ActionResult<List<Email>> GetAll(int creatorId)
        {
            var allEmails = _emailDataService.GetAll(creatorId);
            return Ok(allEmails);
        }

        [HttpGet("{id}")]
        public ActionResult<Email> Get(int id)
        {
            return Ok(_emailDataService.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmailDto email)
        {
            await _emailDataService.AddNewEmailToDbAsync(email);
            return Ok(email);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _emailDataService.SoftDelete(id);
            return Ok();
        }
    }
}