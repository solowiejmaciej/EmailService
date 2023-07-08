using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Repositories
{
    public interface IPushRepository : INotificationRepository
    {
        Task<int> AddAsync(PushNotification push, CancellationToken cancellationToken = default);

        Task<List<PushNotification>> GetAllPushesToUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<PushNotification?> GetPushByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default);

        void ChangePushStatus(int id, EStatus status);
    }

    public sealed class PushRepository : IPushRepository
    {
        private readonly NotificationDbContext _dbContext;

        public PushRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(PushNotification push, CancellationToken cancellationToken = default)
        {
            await _dbContext.PushNotifications.AddAsync(push, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return push.Id;
        }

        public async Task<List<PushNotification>> GetAllPushesToUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.PushNotifications.Where(
                e => e.RecipientId == userId && e.Status != EStatus.ToBeDeleted).
                ToListAsync(cancellationToken);
        }

        public async Task<PushNotification?> GetPushByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.PushNotifications.FirstOrDefaultAsync(
                e =>
                    e.RecipientId == userId &&
                    e.Status != EStatus.ToBeDeleted &&
                    e.Id == id
            , cancellationToken);
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

        public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var pushToBeDeleted = await _dbContext.PushNotifications.FirstOrDefaultAsync(
                e =>
                e.Id == id &&
                e.RecipientId == userId,
                cancellationToken
            );
            if (pushToBeDeleted is null) return 0;
            pushToBeDeleted.Status = EStatus.ToBeDeleted;
            await SaveAsync(cancellationToken);
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

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private bool _disposed;

        private void Dispose(bool disposing)
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