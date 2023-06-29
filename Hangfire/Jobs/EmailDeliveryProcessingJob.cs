using Hangfire;
using Hangfire.Server;
using NotificationService.Entities;

namespace NotificationService.Hangfire.Jobs;

public class EmailDeliveryProcessingJob
{
    private readonly ILogger<EmailDeliveryProcessingJob> _logger;
    public EmailDeliveryProcessingJob(ILogger<EmailDeliveryProcessingJob> logger)
    {
        _logger = logger;
    }

    [JobDisplayName("EmailDeliveryProcessingJob")]
    [Queue(HangfireQueues.HIGH_PRIORITY)]
    public Task Perform(
        int emailId,
        PerformContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("EmailDeliveryProcessingJob performing");
        return Task.CompletedTask;
    }
}

public static partial class BackgroundJobClientExtensions
{
    public static void EnqueueMessageDeliveryProcessing(
        this IBackgroundJobClient backgroundJobClient,
        Email email)
    {
        backgroundJobClient.Enqueue<EmailDeliveryProcessingJob>(x =>
            x.Perform(
                email.Id,
                default!,
                CancellationToken.None));
    }
}