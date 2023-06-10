﻿using EmailService.ApplicationUser;
using EmailService.Entities;

namespace EmailService.Repositories
{
    public interface IEmailsRepository : IDisposable
    {
        List<Email> GetAllEmailsByUser(string userId);

        List<Email> GetAllEmails();

        List<Email> GetAllEmailsByCurrentUser();

        Email? GetEmailById(List<Email> emails, int id);

        List<Email> GetEmailsByCreatorId(List<Email> emails, string creatorId);

        void DeleteEmails();

        void SoftDelete(List<Email> emails, int id);

        void InsertEmail(Email email);

        void Save();
    }

    public class EmailsRepository : IEmailsRepository
    {
        private readonly NotificationDbContext _dbContext;
        private readonly IUserContext _userContext;

        public EmailsRepository(NotificationDbContext dbContext, IUserContext userContext)
        {
            _dbContext = dbContext;
            _userContext = userContext;
        }

        public List<Email> GetAllEmailsByUser(string userId)
        {
            return _dbContext.Emails.Where(e => e.CreatedById == userId && e.EmailStatus != EmailStatus.ToBeDeleted).ToList();
        }

        public List<Email> GetAllEmails()
        {
            return _dbContext.Emails.Where(e => e.EmailStatus != EmailStatus.ToBeDeleted).ToList();
        }

        public List<Email> GetAllEmailsByCurrentUser()
        {
            var currentUserId = _userContext.GetCurrentUser().Id;
            var emails = GetAllEmailsByUser(currentUserId);
            return emails;
        }

        public Email? GetEmailById(List<Email> emails, int id)
        {
            return emails.FirstOrDefault(e => e.Id == id);
        }

        public List<Email> GetEmailsByCreatorId(List<Email> emails, string creatorId)
        {
            return emails.Where(e => e.CreatedById == creatorId).ToList();
        }

        public void DeleteEmails()
        {
            var emailsToDelete = _dbContext.Emails.Where(e => e.EmailStatus == EmailStatus.ToBeDeleted);
            _dbContext.Emails.RemoveRange(emailsToDelete);
            Save();
        }

        public void SoftDelete(List<Email> emails, int id)
        {
            var emailToDelete = emails.FirstOrDefault(e => e.Id == id);
            emailToDelete.EmailStatus = EmailStatus.ToBeDeleted;
            Save();
        }

        public void InsertEmail(Email email)
        {
            _dbContext.Add(email);
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