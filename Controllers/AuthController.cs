using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Queries.GetToken;
using NotificationService.Models.Requests;
using NotificationService.Services.Auth;

namespace AuthService.Controllers
{
    [EnableCors("apiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(AddUserRequest user)
        {
            var command = new CreateNewUserCommand()
            {
                Password = user.Password,
                ConfirmPassword = user.ConfirmPassword,
                DeviceId = user.DeviceId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Firstname = user.Firstname,
                Surname = user.Surname,
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserLoginRequest user)
        {
            var querry = new GetTokenQuerry()
            {
                Email = user.Email,
                Password = user.Password
            };

            var response = await _mediator.Send(querry);
            return Ok(response);
        }

        [HttpPost("Login/QR")]
        public ActionResult LoginViaQr()
        {
            throw new NotImplementedException();
        }
    }
}