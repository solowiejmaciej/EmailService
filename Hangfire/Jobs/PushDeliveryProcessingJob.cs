using Hangfire.Server;
using Hangfire;
using Microsoft.Extensions.Options;
using NotificationService.Entities;
using NotificationService.Models.AppSettings;
using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1.Data;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.Services;
using System.Text.Json;
using NotificationService.Repositories;

namespace NotificationService.Hangfire.Jobs
{
    public class PushDeliveryProcessingJob
    {
        private readonly ILogger<PushDeliveryProcessingJob> _logger;
        private readonly IOptions<GoogleFirebaseConfig> _config;
        private readonly IPushRepository _repository;

        public PushDeliveryProcessingJob(
            ILogger<PushDeliveryProcessingJob> logger,
            IOptions<GoogleFirebaseConfig> config,
            IPushRepository repository
        )
        {
            _logger = logger;
            _config = config;
            _repository = repository;
        }

        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("PushDeliveryProcessingJob")]
        [Queue(HangfireQueues.DEFAULT)]
        public async Task Send(
            PushNotification push,
            PerformContext context,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("PushDeliveryProcessingJob invoked");
            FirebaseCloudMessagingService firebaseCloudMessagingService = new FirebaseCloudMessagingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromJson(JsonSerializer.Serialize(_config.Value))
            });

            var pushNotification = new SendMessageRequest()
            {
                Message = new Message()
                {
                    Token = push.DeviceId,
                    Notification = new Notification()
                    {
                        Title = push.Title,
                        Body = push.Content
                    }
                },
            };
            try
            {
                await firebaseCloudMessagingService.Projects.Messages.Send(pushNotification, $"projects/{_config.Value.project_id}").ExecuteAsync(cancellationToken);
                _repository.ChangePushStatus(push.Id, PushStatus.Send);
                await _repository.SaveAsync();
                _logger.LogInformation($"Push {push.Id} send successfully");
            }
            catch (Exception ex)
            {
                _repository.ChangePushStatus(push.Id, PushStatus.HasErrors);
                await _repository.SaveAsync();
                _logger.LogError("Error occurred while trying to send a push" + ex.Message);
            }
        }
    }
}