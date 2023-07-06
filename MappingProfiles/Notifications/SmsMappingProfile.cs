using AutoMapper;
using NotificationService.Entities.NotificationEntities;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.UserContext;

namespace NotificationService.MappingProfiles.Notifications;

public class SmsMappingProfile : Profile
{
    public SmsMappingProfile(IUserContext userContext)
    {
        var user = userContext.GetCurrentUser();

        CreateMap<SmsNotificationDto, SmsNotification>();
        CreateMap<SmsNotification, SmsNotificationDto>();

        CreateMap<CreateNewSmsCommand, SmsNotification>();
        CreateMap<SmsNotification, CreateNewSmsCommand>();

        CreateMap<GetSmsByIdQuerry, SmsNotificationDto>();
        CreateMap<SmsNotificationDto, GetSmsByIdQuerry>();
    }
}