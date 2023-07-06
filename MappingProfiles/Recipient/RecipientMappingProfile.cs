using AutoMapper;
using NotificationService.Entities;

namespace NotificationService.MappingProfiles.Recipient
{
    public class RecipientMappingProfile : Profile
    {
        public RecipientMappingProfile()
        {
            CreateMap<ApplicationUser, Models.Recipient>()
                .ForMember(u => u.UserId, c => c.MapFrom(r => r.Id));
            CreateMap<Models.Recipient, ApplicationUser>()
                .ForMember(u => u.Id, c => c.MapFrom(r => r.UserId));
        }
    }
}

/*.ForMember(u => u.DeviceId, c => c.MapFrom(r => r.DeviceId))
    .ForMember(u => u.Email, c => c.MapFrom(r => r.Email))
    .ForMember(u => u.PhoneNumber, c => c.MapFrom(r => r.PhoneNumber));*/