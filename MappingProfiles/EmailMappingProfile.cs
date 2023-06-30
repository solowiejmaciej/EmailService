using AuthService.UserContext;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;

namespace NotificationService.MappingProfiles
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile(IUserContext userContext)
        {
            var user = userContext.GetCurrentUser();

            CreateMap<EmailDto, Email>();
            CreateMap<Email, EmailDto>();
            CreateMap<EmailRequest, Email>()
                .ForMember(m => m.EmailFrom, c => c.MapFrom(m => user.Login))
                .ForMember(m => m.CreatedById, c => c.MapFrom(m => user.Id));
            CreateMap<Email, EmailRequest>();
        }
    }
}