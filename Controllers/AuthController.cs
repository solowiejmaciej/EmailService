using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models.Requests;
using NotificationService.Services.Auth;
using NotificationService.Services.Users;

namespace AuthService.Controllers
{
    [EnableCors("apiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTManager _jwtManager;
        private readonly IUserService _userService;

        public AuthController(
            IJWTManager jwtManager,
            IUserService userService
            )
        {
            _jwtManager = jwtManager;
            _userService = userService;
        }

        [HttpPost("Login")]
        public ActionResult Login(UserLoginRequest user)
        {
            var token = _jwtManager.GenerateJWT(user);
            return Ok(token);
        }

        [HttpPost("Register")]
        public ActionResult Register(UserRegisterRequest user)
        {
            var token = _userService.Register(user);
            return Ok(token);
        }

        [HttpPost("Login/QR")]
        public ActionResult LoginViaQr()
        {
            throw new NotImplementedException();
        }
    }
}