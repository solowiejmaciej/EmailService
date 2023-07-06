using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NotificationService.Models.Dtos;

namespace NotificationService.MappingProfiles.User
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDto, IdentityUser>();
            CreateMap<IdentityUser, UserDto>();
        }
    }
}