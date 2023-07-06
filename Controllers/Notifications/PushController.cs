using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.MediatR.Commands;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Commands.Delete;
using NotificationService.MediatR.Queries;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Requests;

namespace NotificationService.Controllers.Notifications
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PushController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PushController(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Add(
            [FromBody] PushRequest request,
            [FromQuery][Required] string userId
            )
        {
            var command = new CreateNewPushCommand()
            {
                Content = request.Content,
                Title = request.Title,
                RecipiantId = userId
            };
            var createdPushId = await _mediator.Send(command);
            return Created($"/api/Push/{createdPushId}", command);
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var querry = new GetAllPushesQuerry()
            {
            };
            var allPushesDtoByCurrentUser = await _mediator.Send(querry);
            return Ok(allPushesDtoByCurrentUser);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var querry = new GetPushByIdQuerry()
            {
                Id = id
            };

            var pushCreatedByCurrentUserWithSearchId = await _mediator.Send(querry);
            return Ok(pushCreatedByCurrentUserWithSearchId);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeletePushCommand()
            {
                Id = id
            };
            await _mediator.Send(command);
            return Accepted();
        }
    }
}