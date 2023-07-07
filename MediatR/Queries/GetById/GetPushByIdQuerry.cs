using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public record GetPushByIdQuerry : IRequest<PushNotificationDto>
    {
        public int Id { get; set; }
    }
}