using Hangfire;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Hangfire.Jobs;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Hangfire.Manager;

public class NotificationJobManager : INotificationJobManager
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NotificationJobManager(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void EnqueueEmailDeliveryDeliveryJob(EmailNotification email, Recipient recipient)
    {
        _backgroundJobClient.Enqueue<EmailDeliveryProcessingJob>(x =>
             x.Send(
                email,
                recipient,
                default!,
                CancellationToken.None));
    }

    public void EnqueuePushDeliveryDeliveryJob(PushNotification push, Recipient recipient)
    {
        _backgroundJobClient.Enqueue<PushDeliveryProcessingJob>(x =>
            x.Send(
                push,
                recipient,
                default!,
                CancellationToken.None));
    }

    public void EnqueueSmsDeliveryDeliveryJob(SmsNotification sms, Recipient recipient)
    {
        _backgroundJobClient.Enqueue<SmsDeliveryProcessingJob>(x =>
            x.Send(
                sms,
                recipient,
                default!,
                CancellationToken.None));
    }
}

public interface INotificationJobManager
{
    void EnqueueEmailDeliveryDeliveryJob(EmailNotification email, Recipient recipient);

    void EnqueueSmsDeliveryDeliveryJob(SmsNotification sms, Recipient recipient);

    void EnqueuePushDeliveryDeliveryJob(PushNotification push, Recipient recipient);
}