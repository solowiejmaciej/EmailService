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
        List<EmailDto> GetAllByCurrentUser();

        EmailDto GetById(int id);

        void SoftDelete(int id);

        Task<int> AddNewEmailToDbAsync(EmailRequest dto);

        List<EmailDto> GetAllEmails();
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

        public List<EmailDto> GetAllByCurrentUser()
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
            var currentUser = _userContext.GetCurrentUser();

            //If user isn't admin, return only this user emails
            if (currentUser.Role != "Admin")
            {
                var userCacheData = _cache.GetData<EmailDto>("user" + id);

                if (userCacheData != null!)
                {
                    _logger.LogInformation("Data fetched from redis");
                    return userCacheData;
                }

                var userEmails = _emailsRepository.GetAllEmailsByCurrentUser();
                var userEmail = _emailsRepository.GetEmailById(userEmails, id);
                if (userEmail == null)
                {
                    throw new NotFoundException($"Email with id {id} not found");
                }

                var userDto = _mapper.Map<EmailDto>(userEmail);

                _cache.SetData("user" + id, userDto, _exipryTime);

                return userDto;
            }
            //else return all emails

            var adminCacheData = _cache.GetData<EmailDto>(id.ToString());

            if (adminCacheData != null!)
            {
                _logger.LogInformation("Data fetched from redis");
                return adminCacheData;
            }

            var adminEmails = _emailsRepository.GetAllEmails();
            var adminEmail = _emailsRepository.GetEmailById(adminEmails, id);
            if (adminEmail == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }

            var adminDto = _mapper.Map<EmailDto>(adminEmail);

            _cache.SetData(id.ToString(), adminDto, _exipryTime);

            return adminDto;
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