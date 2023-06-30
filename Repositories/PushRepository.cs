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

        public void ChangePushStatus(int id, PushStatus status)
        {
            var push = _dbContext.PushNotifications.FirstOrDefault(e => e.Id == id);
            if (push != null) push.Status = status;
            Save();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IPushRepository : IRepository
    {
        Task<int> AddAsync(PushNotification push, UserDto userId);

        PushNotification GetById(int id);

        List<PushNotification> GetByUserId(string id);

        List<PushNotification> GetAll();

        void ChangePushStatus(int id, PushStatus status);
    }
}