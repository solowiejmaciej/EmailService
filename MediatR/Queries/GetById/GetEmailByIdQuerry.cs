using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public record GetEmailByIdQuerry : IRequest<EmailNotificationDto>
    {
        public int Id { get; set; }
    }
}