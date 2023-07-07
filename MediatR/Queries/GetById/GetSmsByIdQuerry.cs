using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public record GetSmsByIdQuerry : IRequest<SmsNotificationDto>
    {
        public int Id { get; set; }
    }
}