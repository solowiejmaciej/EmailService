using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models.Dtos;

namespace NotificationService.MappingProfiles.User
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}