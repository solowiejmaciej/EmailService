using AutoMapper;
using NotificationService.Entities.NotificationEntities;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.UserContext;

namespace NotificationService.MappingProfiles.Notifications;

public class EmailMappingProfile : Profile
{
    public EmailMappingProfile()
    {
        CreateMap<EmailNotificationDto, EmailNotification>();
        CreateMap<EmailNotification, EmailNotificationDto>();

        CreateMap<CreateNewEmailCommand, EmailNotification>();
        CreateMap<EmailNotification, CreateNewEmailCommand>();

        CreateMap<GetEmailByIdQuerry, EmailNotificationDto>();
        CreateMap<EmailNotificationDto, GetEmailByIdQuerry>();
    }
}