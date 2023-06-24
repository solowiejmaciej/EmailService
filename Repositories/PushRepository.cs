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

        public PushNotification GetById(int id)
        {
            var pushById = _dbContext.PushNotifications.FirstOrDefault(p => p.Id == id);
            return pushById;
        }

        public List<PushNotification> GetAll()
        {
            var pushAll = _dbContext.PushNotifications.ToList();
            return pushAll;
        }

        public List<PushNotification> GetByUserId(string id)
        {
            var pushById = _dbContext.PushNotifications.Where(p => p.UserId == id).ToList();
            return pushById;
        }
    }

    public interface IPushRepository
    {
        Task<int> AddAsync(PushNotification push, UserDto userId);

        PushNotification GetById(int id);

        List<PushNotification> GetByUserId(string id);

        List<PushNotification> GetAll();
    }
}