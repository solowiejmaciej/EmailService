using AuthService.UserContext;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Exceptions;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Services
{
    public interface IEmailDataService
    {
        EmailDto GetById(int id);

        List<EmailDto> GetAllEmails();

        void SoftDelete(int id);

        Task<int> AddNewEmailToDbAsync(EmailRequest dto);
    }

    public class EmailService : IEmailDataService
    {
        private readonly NotificationDbContext _dbContext;
        private readonly ILogger<EmailSenderService> _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IUserContext _userContext;
        private readonly IEmailsRepository _emailsRepository;
        private readonly DateTimeOffset _exipryTime;

        public EmailService(
            NotificationDbContext dbContext,
            ILogger<EmailSenderService> logger,
            IMapper mapper,
            ICacheService cache,
            IUserContext userContext,
            IEmailsRepository emailsRepository
            )
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _userContext = userContext;
            _emailsRepository = emailsRepository;
            _dbContext = dbContext;
            _exipryTime = DateTimeOffset.Now.AddMinutes(1);
        }

        public List<EmailDto> GetAllEmails()
        {
            var cacheData = _cache.GetData<List<EmailDto>>("AllEmails");
            if (cacheData != null! && cacheData.Any())
            {
                _logger.LogInformation("Data fetched from redis");
                return cacheData;
            }

            var emails = _emailsRepository.GetAllEmails();
            var emailsDtos = _mapper.Map<List<EmailDto>>(emails);

            _cache.SetData("AllEmails", emailsDtos, _exipryTime);

            return emailsDtos;
        }

        private List<EmailDto> GetAllByCurrentUser()
        {
            var currentUser = _userContext.GetCurrentUser();

            //Check cache data
            var cacheData = _cache.GetData<List<EmailDto>>("Emails" + currentUser.Id);

            //If there is something with the key "Emails+userid", then return it to the user
            if (cacheData != null! && cacheData.Any())
            {
                _logger.LogInformation("Data fetched from redis");
                return cacheData;
            }
            //Set the expiry time and set the data for the future usage

            var emails = _emailsRepository.GetAllEmailsByCurrentUser().ToList();
            var dtos = _mapper.Map<List<EmailDto>>(emails);

            _cache.SetData("Emails" + currentUser.Id, dtos, _exipryTime);

            //return straight from db
            return dtos;
        }

        public EmailDto GetById(int id)
        {
            var cacheData = _cache.GetData<EmailDto>(id.ToString());

            if (cacheData != null!)
            {
                _logger.LogInformation("Data fetched from redis");
                return cacheData;
            }

            var emails = _emailsRepository.GetAllEmails();
            var email = _emailsRepository.GetEmailById(emails, id);
            if (email == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }

            var dto = _mapper.Map<EmailDto>(email);

            _cache.SetData(id.ToString(), dto, _exipryTime);

            return dto;
        }

        public void SoftDelete(int id)
        {
            var emailToDelete = GetAllByCurrentUser().FirstOrDefault(e => e.Id == id);
            if (emailToDelete == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }
            emailToDelete.EmailStatus = EmailStatus.ToBeDeleted;
            _emailsRepository.Save();
            _cache.RemoveData("Emails");
            _logger.LogInformation($"Email with Id {id} marked as deleted");
        }

        public async Task<int> AddNewEmailToDbAsync(EmailRequest dto)
        {
            var email = _mapper.Map<Email>(dto);
            _emailsRepository.InsertEmail(email);
            _cache.RemoveData("Emails" + _userContext.GetCurrentUser().Id);
            return email.Id;
        }
    }
}