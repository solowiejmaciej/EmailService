using AuthService.UserContext;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models;

namespace NotificationService.MappingProfiles
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile(IUserContext userContext)
        {
            var user = userContext.GetCurrentUser();

            CreateMap<EmailDto, Email>()
                .ForMember(m => m.EmailFrom, c => c.MapFrom(m => user.Login))
                .ForMember(m => m.CreatedById, c => c.MapFrom(m => user.Id));
            CreateMap<Email, EmailDto>();
            CreateMap<EmailRequest, Email>();
            CreateMap<Email, EmailRequest>();
        }
    }
}