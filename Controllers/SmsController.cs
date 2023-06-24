using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Add()
        {
            throw new NotImplementedException();
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