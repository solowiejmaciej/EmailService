using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetAll
{
    public class GetAllEmailsQuerry : IRequest<List<EmailNotificationDto>>
    {
    }
}