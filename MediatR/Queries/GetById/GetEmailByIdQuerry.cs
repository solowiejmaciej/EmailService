using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public class GetEmailByIdQuerry : IRequest<EmailNotificationDto>
    {
        public int Id { get; set; }
    }
}