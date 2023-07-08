using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Commands.Delete;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.QueryParameters;
using NotificationService.Models.Requests;

namespace NotificationService.Controllers.Notifications
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Add(
            [FromBody] AddEmailRequest request,
            [FromQuery] EmailRequestQuerryParameters parameters
        )
        {
            var command = new CreateNewEmailCommand()
            {
                Content = request.Content,
                Subject = request.Subject,
                RecipiantId = parameters.UserId
            };

            var createdEmailId = await _mediator.Send(command);
            return Created($"/api/Email/{createdEmailId}", command);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var query = new GetAllEmailsQuerry();
            var emailDtosByCurrentUser = await _mediator.Send(query);
            return Ok(emailDtosByCurrentUser);
        }

        [Authorize]
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var query = new GetEmailByIdQuerry()
            {
                Id = id
            };

            var emailCreatedByCurrentUserWithSearchId = await _mediator.Send(query);
            return Ok(emailCreatedByCurrentUserWithSearchId);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteEmailCommand()
            {
                Id = id
            };
            await _mediator.Send(command);
            return Accepted();
        }
    }
}