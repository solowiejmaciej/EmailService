using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models.Requests;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SmsRequest request)
        {
            await _smsService.AddAsync(request);
            return Ok(request);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? userId)
        {
            throw new NotImplementedException();
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetById([FromRoute] int id)
        {
            throw new NotImplementedException();
        }
    }
}