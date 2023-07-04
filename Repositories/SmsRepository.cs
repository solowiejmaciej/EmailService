using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;

namespace NotificationService.Repositories
{
    public class SmsRepository : ISmsRepository
    {
        private readonly NotificationDbContext _dbContext;

        public SmsRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ChangeSmsStatus(int id, SmsStatus status)
        {
            var sms = _dbContext.SmsNotifications.FirstOrDefault(e => e.Id == id);
            if (sms != null) sms.Status = status;
            Save();
        }

        public async Task AddAsync(Sms sms)
        {
            await _dbContext.AddAsync(sms);
            await SaveAsync();
        }

        public void Add(Sms sms)
        {
            _dbContext.SmsNotifications.Add(sms);
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

        public void DeleteInBackground()
        {
            var smsesToDelete = _dbContext.SmsNotifications.Where(e => e.Status == SmsStatus.ToBeDeleted);
            _dbContext.SmsNotifications.RemoveRange(smsesToDelete);
            Save();
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

    public interface ISmsRepository : IRepository
    {
        void ChangeSmsStatus(int id, SmsStatus status);

        Task AddAsync(Sms sms);

        void Add(Sms sms);
    }
}