using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Repositories
{
    public interface IEmailsRepository : INotificationRepository
    {
        Task<int> AddAsync(EmailNotification email, CancellationToken cancellationToken = default);

        Task<List<EmailNotification>> GetAllEmailsToUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<EmailNotification?> GetEmailByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default);

        void ChangeEmailStatus(int id, EStatus status);
    }

    public sealed class EmailsRepository : IEmailsRepository
    {
        private readonly NotificationDbContext _dbContext;

        public EmailsRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(EmailNotification email, CancellationToken cancellationToken = default)
        {
            var newEmail = await _dbContext.AddAsync(email, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return newEmail.Entity.Id;
        }

        public async Task<List<EmailNotification>> GetAllEmailsToUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.EmailsNotifications.Where(
                    e => e.RecipientId == userId && e.Status != EStatus.ToBeDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<EmailNotification?> GetEmailByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.EmailsNotifications.FirstOrDefaultAsync(
                e =>
                    e.RecipientId == userId &&
                    e.Status != EStatus.ToBeDeleted &&
                    e.Id == id,
                cancellationToken
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

        public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var emailToDeleted = await _dbContext.EmailsNotifications.FirstOrDefaultAsync(e =>
                e.Id == id &&
                e.RecipientId == userId,
                cancellationToken
            );
            if (emailToDeleted is null) return 0;
            emailToDeleted.Status = EStatus.ToBeDeleted;
            await SaveAsync(cancellationToken);
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