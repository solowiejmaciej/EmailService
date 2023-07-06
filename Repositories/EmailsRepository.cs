using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Exceptions;
using NotificationService.UserContext;

namespace NotificationService.Repositories
{
    public interface IEmailsRepository : INotificationRepository
    {
        Task<int> AddAsync(EmailNotification email);

        Task<List<EmailNotification>> GetAllEmailsToUserIdAsync(string userId);

        Task<EmailNotification?> GetEmailByIdAndUserIdAsync(int id, string userId);

        void ChangeEmailStatus(int id, EStatus status);
    }

    public class EmailsRepository : IEmailsRepository
    {
        private readonly NotificationDbContext _dbContext;

        public EmailsRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(EmailNotification email)
        {
            var newEmail = await _dbContext.AddAsync(email);
            await _dbContext.SaveChangesAsync();
            return newEmail.Entity.Id;
        }

        public async Task<List<EmailNotification>> GetAllEmailsToUserIdAsync(string userId)
        {
            return await _dbContext.EmailsNotifications.Where(e => e.RecipientId == userId && e.Status != EStatus.ToBeDeleted).ToListAsync();
        }

        public async Task<EmailNotification?> GetEmailByIdAndUserIdAsync(int id, string userId)
        {
            return await _dbContext.EmailsNotifications.FirstOrDefaultAsync(
                e =>
                    e.RecipientId == userId &&
                    e.Status != EStatus.ToBeDeleted &&
                    e.Id == id
            );
        }

        public void ChangeEmailStatus(int id, EStatus status)
        {
            var email = _dbContext.EmailsNotifications.FirstOrDefault(e => e.Id == id);
            if (email != null) email.Status = status;
            Save();
        }

        public void DeleteInBackground()
        {
            var emailsToDelete = _dbContext.EmailsNotifications.Where(e => e.Status == EStatus.ToBeDeleted);
            _dbContext.EmailsNotifications.RemoveRange(emailsToDelete);
            Save();
        }

        public async Task<int> SoftDeleteAsync(int id, string userId)
        {
            var emailToDeleted = await _dbContext.EmailsNotifications.FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.RecipientId == userId
            );
            if (emailToDeleted is null) return 0;
            emailToDeleted.Status = EStatus.ToBeDeleted;
            await SaveAsync();
            return emailToDeleted.Id;
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