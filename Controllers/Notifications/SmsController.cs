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
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SmsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] AddSmsRequest request,
            [FromQuery] SmsRequestQuerryParameters parameters
        )
        {
            var command = new CreateNewSmsCommand()
            {
                Content = request.Content,
                RecipiantId = parameters.UserId
            };

            var createdSmsId = await _mediator.Send(command);
            return Created($"/api/Sms/{createdSmsId}", command);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var querry = new GetAllSmsQuerry()
            {
            };
            var allSmsDtoByCurrentUser = await _mediator.Send(querry);
            return Ok(allSmsDtoByCurrentUser);
        }

        [Authorize]
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var querry = new GetSmsByIdQuerry()
            {
                Id = id
            };

            var smsCreatedByCurrentUserWithSearchId = await _mediator.Send(querry);
            return Ok(smsCreatedByCurrentUserWithSearchId);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteSmsCommand()
            {
                Id = id
            };
            await _mediator.Send(command);
            return Accepted();
        }
    }
}