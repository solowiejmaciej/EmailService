using AutoMapper;
using NotificationService.Entities.NotificationEntities;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.UserContext;

namespace NotificationService.MappingProfiles.Notifications;

public class PushMappingProfile : Profile
{
    public PushMappingProfile(IUserContext userContext)
    {
        var user = userContext.GetCurrentUser();

        CreateMap<PushNotification, PushNotificationDto>();
        CreateMap<PushNotificationDto, PushNotificationDto>();

        CreateMap<CreateNewPushCommand, PushNotification>();
        CreateMap<PushNotification, CreateNewPushCommand>();

        CreateMap<GetPushByIdQuerry, PushNotificationDto>();
        CreateMap<PushNotificationDto, GetPushByIdQuerry>();
    }
}