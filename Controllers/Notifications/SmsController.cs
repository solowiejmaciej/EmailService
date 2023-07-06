using System.ComponentModel.DataAnnotations;
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
using NotificationService.Services.Notifications;

namespace NotificationService.Controllers.Notifications
{
    [ApiController]
    //[Authorize]
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
            [FromBody] SmsRequest request,
            [FromQuery][Required] string userId
        )
        {
            var command = new CreateNewSmsCommand()
            {
                Content = request.Content,
                RecipiantId = userId
            };

            var createdSmsId = await _mediator.Send(command);
            return Created($"/api/Sms/{createdSmsId}", command);
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var querry = new GetAllSmsQuerry()
            {
            };
            var allSmsDtoByCurrentUser = await _mediator.Send(querry);
            return Ok(allSmsDtoByCurrentUser);
        }

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