using AuthService.Models;
using NotificationService.Entities;

namespace NotificationService.Repositories
{
    public class PushRepository : IPushRepository
    {
        private readonly NotificationDbContext _dbContext;

        public PushRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(PushNotification push, UserDto userDto)
        {
            var pushFromDb = await _dbContext.PushNotifications.AddAsync(push);
            pushFromDb.Entity.UserId = userDto.Id;
            pushFromDb.Entity.DeviceId = userDto.DeviceId;
            await _dbContext.SaveChangesAsync();
            return push.Id;
        }
    }

    public interface IPushRepository
    {
        Task<int> AddAsync(PushNotification push, UserDto userId);
    }
}