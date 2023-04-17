using AutoMapper;
using EmailService.Entities;
using EmailService.Models;
using Microsoft.Extensions.Options;

namespace EmailService.MappingProfiles
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile()
        {
            CreateMap<EmailDto, Email>()
                .ForMember(m => m.EmailFrom, c => c.MapFrom(m => "solowiejmaciej@gmail.com"));
            CreateMap<Email, EmailDto>();
        }
    }
}