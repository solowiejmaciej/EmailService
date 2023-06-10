using AutoMapper;
using EmailService.ApplicationUser;
using EmailService.Entities;
using EmailService.Models;

namespace EmailService.MappingProfiles
{
    public class PushMappingProfile : Profile
    {
        public PushMappingProfile(IUserContext userContext)
        {
            var user = userContext.GetCurrentUser();

            CreateMap<PushNotification, PushNotificationDto>();
            CreateMap<PushNotificationDto, PushNotificationDto>();
            CreateMap<PushNotification, PushRequest>()
                .ForMember(p => p.PushTitle, c => c.MapFrom(p => p.Title))
                .ForMember(p => p.PushContent, c => c.MapFrom(p => p.Content));
            CreateMap<PushRequest, PushNotification>()
                .ForMember(p => p.Content, c => c.MapFrom(p => p.PushContent))
                .ForMember(p => p.Title, c => c.MapFrom(p => p.PushTitle));
        }
    }
}