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
    [Authorize]
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
            var querry = new GetAllUsersQuerry()
            {
            };
            var result = await _mediator.Send(querry);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById([FromRoute] string id)
        {
            var querry = new GetUserByIdQuerry()
            {
                Id = id
            };

            var result = await _mediator.Send(querry);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(
            [FromRoute] string id,
            [FromBody] UpdateUserRequest request
            )
        {
            var querry = new UpdateUserCommand()
            {
                Id = id,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DeviceId = request.DeviceId,
                Firstname = request.Firstname,
                Surname = request.Surname,
            };

            var updatedUserDto = await _mediator.Send(querry);
            return Ok(updatedUserDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var querry = new DeleteUserCommand()
            {
                Id = id,
            };

            await _mediator.Send(querry);
            return Accepted();
        }
    }
}