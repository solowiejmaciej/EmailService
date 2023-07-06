using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Repositories
{
    public interface IPushRepository : INotificationRepository
    {
        Task<int> AddAsync(PushNotification push);

        Task<List<PushNotification>> GetAllPushesToUserIdAsync(string userId);

        Task<PushNotification?> GetPushByIdAndUserIdAsync(int id, string userId);

        void ChangePushStatus(int id, EStatus status);
    }

    public class PushRepository : IPushRepository
    {
        private readonly NotificationDbContext _dbContext;

        public PushRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(PushNotification push)
        {
            await _dbContext.PushNotifications.AddAsync(push);
            await _dbContext.SaveChangesAsync();
            return push.Id;
        }

        public async Task<List<PushNotification>> GetAllPushesToUserIdAsync(string userId)
        {
            return await _dbContext.PushNotifications.Where(e => e.RecipientId == userId && e.Status != EStatus.ToBeDeleted).ToListAsync();
        }

        public async Task<PushNotification?> GetPushByIdAndUserIdAsync(int id, string userId)
        {
            return await _dbContext.PushNotifications.FirstOrDefaultAsync(
                e =>
                    e.RecipientId == userId &&
                    e.Status != EStatus.ToBeDeleted &&
                    e.Id == id
            );
        }

        public void ChangePushStatus(int id, EStatus status)
        {
            var push = _dbContext.PushNotifications.FirstOrDefault(e => e.Id == id);
            if (push != null) push.Status = status;
            Save();
        }

        public void DeleteInBackground()
        {
            var pushesToDelete = _dbContext.PushNotifications.Where(e => e.Status == EStatus.ToBeDeleted);
            _dbContext.PushNotifications.RemoveRange(pushesToDelete);
            Save();
        }

        public async Task<int> SoftDeleteAsync(int id, string userId)
        {
            var pushToBeDeleted = await _dbContext.PushNotifications.FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.RecipientId == userId
            );
            if (pushToBeDeleted is null) return 0;
            pushToBeDeleted.Status = EStatus.ToBeDeleted;
            await SaveAsync();
            return pushToBeDeleted.Id;
        }

        public void SoftDelete(int id)
        {
            throw new NotImplementedException();
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
}