using Hangfire;
using NotificationService.Entities;
using NotificationService.Hangfire.Jobs;

namespace NotificationService.Hangfire.Manager;

public class NotificationJobManager : INotificationJobManager
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NotificationJobManager(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void EnqueueEmailDeliveryDeliveryJob(Email email)
    {
        _backgroundJobClient.Enqueue<EmailDeliveryProcessingJob>(x =>
             x.Send(
                email,
                default!,
                CancellationToken.None));
    }

    public void EnqueuePushDeliveryDeliveryJob(PushNotification push)
    {
        _backgroundJobClient.Enqueue<PushDeliveryProcessingJob>(x =>
            x.Send(
                push,
                default!,
                CancellationToken.None));
    }

    public void EnqueueSmsDeliveryDeliveryJob(Sms sms)
    {
        _backgroundJobClient.Enqueue<SmsDeliveryProcessingJob>(x =>
            x.Send(
                sms,
                default!,
                CancellationToken.None));
    }
}

public interface INotificationJobManager
{
    void EnqueueEmailDeliveryDeliveryJob(Email email);

    void EnqueueSmsDeliveryDeliveryJob(Sms sms);

    void EnqueuePushDeliveryDeliveryJob(PushNotification push);
}