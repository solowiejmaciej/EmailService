using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NotificationService.MediatR.Commands.Delete;
using NotificationService.MediatR.Commands.Update;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Requests.Update;

namespace NotificationService.Controllers
{
    [EnableCors("apiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuerry());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuerry()
            {
                Id = id
            });
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(
            [FromRoute] string id,
            [FromBody] UpdateUserRequest request
            )
        {
            var updateUserCommand = new UpdateUserCommand()
            {
                Id = id,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DeviceId = request.DeviceId,
                Firstname = request.Firstname,
                Surname = request.Surname,
            };

            var updatedUserDto = await _mediator.Send(updateUserCommand);
            return Ok(updatedUserDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _mediator.Send(new DeleteUserCommand()
            {
                Id = id,
            });
            return Accepted();
        }
    }
}