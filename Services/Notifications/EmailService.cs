using AutoMapper;
using NotificationService.Hangfire.Manager;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.Services.Notifications
{
    public interface IEmailDataService
    {
        /*        EmailNotificationDto GetById(int id);

                List<EmailNotificationDto> GetAllEmails();

                void SoftDelete(int id);*/
    }

    public class EmailService : IEmailDataService
    {
        private readonly ILogger<IEmailDataService> _logger;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IUserContext _userContext;
        private readonly IEmailsRepository _emailsRepository;
        private readonly DateTimeOffset _exipryTime;
        private readonly INotificationJobManager _notificationJobManager;

        public EmailService(
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
            _exipryTime = DateTimeOffset.Now.AddMinutes(1);
            _notificationJobManager = notificationJobManager;
        }

        /*        public List<EmailNotificationDto> GetAllEmails()
                {
                    var cacheData = _cache.GetData<List<EmailNotificationDto>>("AllEmails");
                    if (cacheData != null! && cacheData.Any())
                    {
                        _logger.LogInformation("Data fetched from redis");
                        return cacheData;
                    }

                    //var emails = _emailsRepository.GetAllEmails();
                    // var emailsDtos = _mapper.Map<List<EmailNotificationDto>>(emails);

                    _cache.SetData("AllEmails", emailsDtos, _exipryTime);

                    return emailsDtos;
                }

                private List<EmailNotificationDto> GetAllByCurrentUser()
                {
                    var currentUser = _userContext.GetCurrentUser();

                    //Check cache data
                    var cacheData = _cache.GetData<List<EmailNotificationDto>>("Emails" + currentUser.Id);

                    //If there is something with the key "Emails+userid", then return it to the user
                    if (cacheData != null! && cacheData.Any())
                    {
                        _logger.LogInformation("Data fetched from redis");
                        return cacheData;
                    }
                    //Set the expiry time and set the data for the future usage

                    //var emails = _emailsRepository.GetAllEmailsToCurrentUser().ToList();
                    //var dtos = _mapper.Map<List<EmailNotificationDto>>(emails);

                    _cache.SetData("Emails" + currentUser.Id, dtos, _exipryTime);

                    //return straight from db
                    return dtos;
                }

                public EmailNotificationDto GetById(int id)
                {
                    var cacheData = _cache.GetData<EmailNotificationDto>(id.ToString());

                    if (cacheData != null!)
                    {
                        _logger.LogInformation("Data fetched from redis");
                        return cacheData;
                    }

                    //var emails = _emailsRepository.GetAllEmails();
                    //var email = _emailsRepository.GetEmailById(emails, id);
                    if (email == null)
                    {
                        throw new NotFoundException($"Email with id {id} not found");
                    }

                    var dto = _mapper.Map<EmailNotificationDto>(email);

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
                    var emailToDelete = _mapper.Map<EmailNotification>(emailToDeleteDtos);
                    _emailsRepository.SoftDelete(emailToDelete);
                    _cache.RemoveData("Emails");
                    _logger.LogInformation($"Email with Id {id} marked as deleted");
                }

                public Task<int> AddNewEmailToDbAsync(EmailRequest request)
                {
                    var email = _mapper.Map<EmailNotification>(request);
                    _emailsRepository.InsertEmail(email);
                    _emailsRepository.Save();
                    _notificationJobManager.EnqueueEmailDeliveryDeliveryJob(email);
                    _cache.RemoveData("Emails" + _userContext.GetCurrentUser().Id);
                    return Task.FromResult(email.Id);
                }
            }*/
    }
}