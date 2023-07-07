using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models;

namespace NotificationService.MappingProfiles.Recipients
{
    public class RecipientMappingProfile : Profile
    {
        public RecipientMappingProfile()
        {
            CreateMap<ApplicationUser, Recipient>()
                .ForMember(u => u.UserId, c => c.MapFrom(r => r.Id));
            CreateMap<Recipient, ApplicationUser>()
                .ForMember(u => u.Id, c => c.MapFrom(r => r.UserId));
        }
    }
}