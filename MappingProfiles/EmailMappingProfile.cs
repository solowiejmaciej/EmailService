using AuthService;
using AutoMapper;
using EmailService.Entities;
using EmailService.Models;

namespace EmailService.MappingProfiles
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile(IUserContext userContext)
        {
            var user = userContext.GetCurrentUser();

            CreateMap<EmailDto, Email>()
                .ForMember(m => m.EmailFrom, c => c.MapFrom(m => "solowiejmaciej@gmail.com"))
                .ForMember(m => m.CreatedById, c => c.MapFrom(m => user.Id));
            CreateMap<Email, EmailDto>();
        }
    }
}