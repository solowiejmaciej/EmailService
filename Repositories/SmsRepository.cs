using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Repositories
{
    public interface ISmsRepository : INotificationRepository
    {
        Task AddAsync(SmsNotification sms);

        void Add(SmsNotification sms);

        Task<List<SmsNotification>> GetAllSmsToUserIdAsync(string userId);

        Task<SmsNotification?> GetSmsByIdAndUserIdAsync(int id, string userId);

        void ChangeSmsStatus(int id, EStatus status);
    }

    public class SmsRepository : ISmsRepository
    {
        private readonly NotificationDbContext _dbContext;

        public SmsRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SmsNotification sms)
        {
            await _dbContext.AddAsync(sms);
            await SaveAsync();
        }

        public void Add(SmsNotification sms)
        {
            _dbContext.SmsNotifications.Add(sms);
            Save();
        }

        public async Task<List<SmsNotification>> GetAllSmsToUserIdAsync(string userId)
        {
            return await _dbContext.SmsNotifications.Where(e => e.RecipientId == userId && e.Status != EStatus.ToBeDeleted).ToListAsync();
        }

        public async Task<SmsNotification?> GetSmsByIdAndUserIdAsync(int id, string userId)
        {
            return await _dbContext.SmsNotifications.FirstOrDefaultAsync(
                e =>
                    e.RecipientId == userId &&
                    e.Status != EStatus.ToBeDeleted &&
                    e.Id == id
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

        public async Task<int> SoftDeleteAsync(int id, string userId)
        {
            var smsToDeleted = await _dbContext.SmsNotifications.FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.RecipientId == userId
            );
            if (smsToDeleted is null) return 0;
            smsToDeleted.Status = EStatus.ToBeDeleted;
            await SaveAsync();
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