using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetAll
{
    public record GetAllEmailsQuerry : IRequest<List<EmailNotificationDto>>
    {
    }
}