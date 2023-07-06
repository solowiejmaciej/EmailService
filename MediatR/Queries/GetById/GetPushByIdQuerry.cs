using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public class GetPushByIdQuerry : IRequest<PushNotificationDto>
    {
        public int Id { get; set; }
    }
}