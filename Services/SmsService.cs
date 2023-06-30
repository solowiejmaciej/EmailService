using AutoMapper;
using Hangfire;
using NotificationService.Entities;
using NotificationService.Hangfire.Manager;
using NotificationService.Models.Requests;
using NotificationService.Repositories;

namespace NotificationService.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISmsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<SmsService> _logger;
        private readonly INotificationJobManager _jobManager;

        public SmsService(
            ISmsRepository repository,
            IMapper mapper,
            ILogger<SmsService> logger,
            INotificationJobManager jobManager
        )
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _jobManager = jobManager;
        }

        public void Add(SmsRequest request)
        {
            var sms = _mapper.Map<Sms>(request);
            _repository.Add(sms);
        }

        public async Task AddAsync(SmsRequest request)
        {
            var sms = _mapper.Map<Sms>(request);
            await _repository.AddAsync(sms);
            _jobManager.EnqueueSmsDeliveryDeliveryJob(sms);
        }

        public void ChangeSmsStatus(int smsId, SmsStatus smsStatus)
        {
            _repository.ChangeSmsStatus(smsId, smsStatus);
        }
    }

    public interface ISmsService
    {
        void Add(SmsRequest request);

        Task AddAsync(SmsRequest request);

        void ChangeSmsStatus(int smsId, SmsStatus smsStatus);
    }
}