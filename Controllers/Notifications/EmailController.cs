﻿using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Commands.Delete;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Requests;

namespace NotificationService.Controllers.Notifications
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Add(
            [FromBody] EmailRequest request,
            [FromQuery][Required] string userId
        )
        {
            var command = new CreateNewEmailCommand()
            {
                Content = request.Content,
                Subject = request.Subject,
                RecipiantId = userId
            };

            var createdEmailId = await _mediator.Send(command);
            return Created($"/api/Email/{createdEmailId}", command);
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var querry = new GetAllEmailsQuerry()
            {
            };
            var allEmailsDtosByCurrentUser = await _mediator.Send(querry);
            return Ok(allEmailsDtosByCurrentUser);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var querry = new GetEmailByIdQuerry()
            {
                Id = id
            };

            var emailCreatedByCurrentUserWithSearchId = await _mediator.Send(querry);
            return Ok(emailCreatedByCurrentUserWithSearchId);
        }

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