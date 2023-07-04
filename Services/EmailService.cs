using AuthService.UserContext;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Exceptions;
using NotificationService.Hangfire.Manager;
using NotificationService.Models;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;
using NotificationService.Repositories;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace NotificationService.Services
{
    public interface IEmailDataService
    {
        EmailDto GetById(int id);

        List<EmailDto> GetAllEmails();

        void SoftDelete(int id);

        Task<int> AddNewEmailToDbAsync(EmailRequest dto);

        void ChangeEmailStatus(int id, EmailStatus status);
    }

    public class EmailService : IEmailDataService
    {
        private readonly NotificationDbContext _dbContext;
        private readonly ILogger<IEmailDataService> _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IUserContext _userContext;
        private readonly IEmailsRepository _emailsRepository;
        private readonly DateTimeOffset _exipryTime;
        private readonly INotificationJobManager _notificationJobManager;

        public EmailService(
            NotificationDbContext dbContext,
            ILogger<IEmailDataService> logger,
            IMapper mapper,
            ICacheService cache,
            IUserContext userContext,
            IEmailsRepository emailsRepository,
            INotificationJobManager notificationJobManager
            )
        {
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _userContext = userContext;
            _emailsRepository = emailsRepository;
            _dbContext = dbContext;
            _exipryTime = DateTimeOffset.Now.AddMinutes(1);
            _notificationJobManager = notificationJobManager;
        }

        public List<EmailDto> GetAllEmails()
        {
            var exampleMail = new Email()
            {
                EmailFrom = "_config.EmailFrom",
                Body = "This is email send by invoking job",
                EmailTo = "maciejsol1926@gmail.com",
                Subject = "Test email from invoking hangfire job",
                EmailSenderName = "Test email",
                CreatedById = new Guid().ToString(),
                EmailStatus = EmailStatus.New,
                Id = 500,
            };
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
            var emailToDeleteDtos = GetAllByCurrentUser().FirstOrDefault(e => e.Id == id);
            if (emailToDeleteDtos == null)
            {
                throw new NotFoundException($"Email with id {id} not found");
            }
            var emailToDelete = _mapper.Map<Email>(emailToDeleteDtos);
            _emailsRepository.SoftDelete(emailToDelete);
            _cache.RemoveData("Emails");
            _logger.LogInformation($"Email with Id {id} marked as deleted");
        }

        public Task<int> AddNewEmailToDbAsync(EmailRequest request)
        {
            var email = _mapper.Map<Email>(request);
            _emailsRepository.InsertEmail(email);
            _emailsRepository.Save();
            _notificationJobManager.EnqueueEmailDeliveryDeliveryJob(email);
            _cache.RemoveData("Emails" + _userContext.GetCurrentUser().Id);
            return Task.FromResult(email.Id);
        }

        public void ChangeEmailStatus(int id, EmailStatus status)
        {
            _emailsRepository.ChangeEmailStatus(id, status);
        }
    }
}