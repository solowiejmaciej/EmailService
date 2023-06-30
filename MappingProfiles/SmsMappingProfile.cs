using AuthService.UserContext;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;

namespace NotificationService.MappingProfiles
{
    public class SmsMappingProfile : Profile
    {
        public SmsMappingProfile(IUserContext userContext)
        {
            var user = userContext.GetCurrentUser();

            CreateMap<SmsDto, Sms>();
            CreateMap<Sms, SmsRequest>();
            CreateMap<SmsRequest, Sms>()
                .ForMember(m => m.CreatedById, c => c.MapFrom(s => user.Id));
        }
    }
}