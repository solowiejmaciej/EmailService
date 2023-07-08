using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Repositories
{
    public interface ISmsRepository : INotificationRepository
    {
        Task AddAsync(SmsNotification sms, CancellationToken cancellationToken = default);
        
        Task<List<SmsNotification>> GetAllSmsToUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<SmsNotification?> GetSmsByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default);

        void ChangeSmsStatus(int id, EStatus status);
    }

    public sealed class SmsRepository : ISmsRepository
    {
        private readonly NotificationDbContext _dbContext;

        public SmsRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SmsNotification sms, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(sms, cancellationToken);
            await SaveAsync(cancellationToken);
        }

        public void Add(SmsNotification sms)
        {
            _dbContext.SmsNotifications.Add(sms);
            Save();
        }

        public async Task<List<SmsNotification>> GetAllSmsToUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SmsNotifications.Where(
                e => e.RecipientId == userId && e.Status != EStatus.ToBeDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<SmsNotification?> GetSmsByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SmsNotifications.FirstOrDefaultAsync(
                e =>
                    e.RecipientId == userId &&
                    e.Status != EStatus.ToBeDeleted &&
                    e.Id == id,
                cancellationToken
            );
        }

        public void ChangeSmsStatus(int id, EStatus status)
        {
            var sms = _dbContext.SmsNotifications.FirstOrDefault(e => e.Id == id);
            if (sms != null) sms.Status = status;
            Save();
        }

        public void DeleteInBackground()
        {
            var smsesToDelete = _dbContext.SmsNotifications.Where(e => e.Status == EStatus.ToBeDeleted);
            _dbContext.SmsNotifications.RemoveRange(smsesToDelete);
            Save();
        }

        public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var smsToDeleted = await _dbContext.SmsNotifications.FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.RecipientId == userId,
                cancellationToken
            );
            if (smsToDeleted is null) return 0;
            smsToDeleted.Status = EStatus.ToBeDeleted;
            await SaveAsync(cancellationToken);
            return smsToDeleted.Id;
        }

        public void SoftDelete(int id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
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