using AuthService.Services;
using AuthService.Models;

namespace EmailService.Repositories
{
    public class DeviceUserRepository : IDeviceUserRepository
    {
        private readonly IUserService _userService;

        public DeviceUserRepository(IUserService userService)
        {
            _userService = userService;
        }

        public UserDto GetUser(string userId)
        {
            return _userService.GetById(userId);
        }
    }

    public interface IDeviceUserRepository
    {
        UserDto GetUser(string deviceId);
    }
}