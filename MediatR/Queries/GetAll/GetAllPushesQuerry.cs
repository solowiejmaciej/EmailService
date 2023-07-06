using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetAll
{
    public class GetAllPushesQuerry : IRequest<List<PushNotificationDto>>
    {
    }
}