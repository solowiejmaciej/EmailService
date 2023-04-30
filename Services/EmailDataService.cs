using AutoMapper;
using EmailService.Entities;
using EmailService.Exceptions;
using EmailService.Models;

namespace EmailService.Services
{
    public interface IEmailDataService
    {
        List<Email> GetAll(int creatorId);

        Email GetById(int id);

        void SoftDelete(int id);

        Task<int> AddNewEmailToDbAsync(EmailDto dto);
    }

    public class EmailDataService : IEmailDataService
    {
        private readonly EmailsDbContext _dbContext;
        private readonly ILogger<EmailSenderService> _logger;
        private readonly IMapper _mapper;

        public EmailDataService(EmailsDbContext dbContext, ILogger<EmailSenderService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        private IQueryable<Email> GetAllEmails()
        {
            return _dbContext.Emails.Where(e => e.IsDeleted == false);
        }

        public List<Email> GetAll(int creatorId)
        {
            if (creatorId == 0)
            {
                var allEmailsFromDb = GetAllEmails().ToList();
                return allEmailsFromDb;
            }
            else
            {
                return GetByCreatorId(creatorId);
            }
        }

        public Email GetById(int id)
        {
            var email = GetAllEmails().FirstOrDefault(x => x.Id == id);
            if (email == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }
            return email;
        }

        private List<Email> GetByCreatorId(int creatorId)
        {
            var creator = _dbContext.Users.FirstOrDefault(u => u.Id == creatorId);
            if (creator == null)
            {
                throw new NotFoundException($"User with id {creatorId} not found");
            }
            var emails = GetAllEmails().Where(e => e.CreatedById == creatorId).ToList();
            return emails;
        }

        public void SoftDelete(int id)
        {
            var emailToDelete = GetAllEmails().FirstOrDefault(e => e.Id == id);
            if (emailToDelete == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }
            emailToDelete.IsDeleted = true;
            _dbContext.SaveChanges();
            _logger.LogInformation($"Email with Id {id} marked as deleted");
        }

        public async Task<int> AddNewEmailToDbAsync(EmailDto dto)
        {
            var email = _mapper.Map<Email>(dto);
            await _dbContext.AddAsync(email);
            await _dbContext.SaveChangesAsync();
            return email.Id;
        }
    }
}