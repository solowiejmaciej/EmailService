using AuthService;
using AutoMapper;
using EmailService.Entities;
using EmailService.Exceptions;
using EmailService.Models;

namespace EmailService.Services
{
    public interface IEmailDataService
    {
        List<Email> GetAllByCurrentUser();

        Email GetById(int id);

        void SoftDelete(int id);

        Task<int> AddNewEmailToDbAsync(EmailDto dto);
    }

    public class EmailDataService : IEmailDataService
    {
        private readonly EmailsDbContext _dbContext;
        private readonly ILogger<EmailSenderService> _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IUserContext _userContext;

        public EmailDataService(EmailsDbContext dbContext,
            ILogger<EmailSenderService> logger,
            IMapper mapper,
            ICacheService cache,
            IUserContext userContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _userContext = userContext;
        }

        public List<Email> GetAllEmails()
        {
            var emails = _dbContext.Emails.Where(e => e.EmailStatus != EmailStatus.ToBeDeleted).ToList();
            return emails;
        }

        public List<Email> GetAllByCurrentUser()
        {
            var currentUser = _userContext.GetCurrentUser();

            //Check cache data
            var cacheData = _cache.GetData<List<Email>>("Emails" + currentUser.Id);

            //If there is something with the key "Emails", then return it to the user
            if (cacheData != null && cacheData.Any())
            {
                _logger.LogInformation("Data fetched from redis");
                return cacheData;
            }
            //Set the expiry time and set the data for the future usage
            var expiryTime = DateTimeOffset.Now.AddMinutes(1);

            var emails = _dbContext.Emails.Where(e => e.EmailStatus != EmailStatus.ToBeDeleted && e.CreatedById == currentUser.Id).ToList();

            _cache.SetData<List<Email>>("Emails" + currentUser.Id, emails, expiryTime);

            //return straight from db
            return emails;
        }

        public Email GetById(int id)
        {
            var email = GetAllByCurrentUser().FirstOrDefault(x => x.Id == id);
            if (email == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }
            return email;
        }

        private List<Email> GetByCreatorId(string creatorId)
        {
            var creator = _dbContext.Users.FirstOrDefault(u => u.Id == creatorId);
            if (creator == null)
            {
                throw new NotFoundException($"User with id {creatorId} not found");
            }
            var emails = GetAllEmails().Where(e => e.CreatedById.Equals(creatorId)).ToList();
            return emails;
        }

        public void SoftDelete(int id)
        {
            var emailToDelete = GetAllByCurrentUser().FirstOrDefault(e => e.Id == id);
            if (emailToDelete == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }
            emailToDelete.EmailStatus = EmailStatus.ToBeDeleted;
            _dbContext.SaveChanges();
            _cache.RemoveData("Emails");
            _logger.LogInformation($"Email with Id {id} marked as deleted");
        }

        public async Task<int> AddNewEmailToDbAsync(EmailDto dto)
        {
            var email = _mapper.Map<Email>(dto);
            await _dbContext.AddAsync(email);
            await _dbContext.SaveChangesAsync();
            _cache.RemoveData("Emails" + _userContext.GetCurrentUser().Id);
            return email.Id;
        }
    }
}