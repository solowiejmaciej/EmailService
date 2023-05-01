using EmailService.Entities;

namespace EmailService.Repositories
{
    public interface IEmailsRepository : IDisposable
    {
        List<Email> GetAllEmails();

        Email GetEmailById(int id);

        Email GetEmailByCreatorId(int creatorId);

        void DeleteEmails();

        void DeleteEmail(int id);

        void InsertEmail(Email email);

        void Save();
    }

    public class EmailsRepository : IEmailsRepository
    {
        private readonly EmailsDbContext _dbContext;

        public EmailsRepository(EmailsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Email> GetAllEmails()
        {
            throw new NotImplementedException();
        }

        public Email GetEmailById(int id)
        {
            throw new NotImplementedException();
        }

        public Email GetEmailByCreatorId(int creatorId)
        {
            throw new NotImplementedException();
        }

        public void DeleteEmails()
        {
            var emailsToDelete = _dbContext.Emails.Where(e => e.IsDeleted == true);
            _dbContext.Emails.RemoveRange(emailsToDelete);
            Save();
        }

        public void DeleteEmail(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertEmail(Email email)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
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