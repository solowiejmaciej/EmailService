using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EmailService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("GetToken")]
        public ActionResult GetToken(UserDto user)
        {
            var token = _authService.GenerateJWT(user);
            return Ok(token);
        }
        [Authorize]
        [HttpPost("AddNewUser")]
        public ActionResult AddUser(UserDto user)
        {
            _authService.AddNewUser(user);
            return NoContent();
        }

    }
}
