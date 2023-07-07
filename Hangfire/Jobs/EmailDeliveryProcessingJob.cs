using Hangfire;
using Hangfire.Server;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Exceptions;
using NotificationService.Models;
using NotificationService.Models.AppSettings;
using NotificationService.Repositories;
using NotificationService.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace NotificationService.Hangfire.Jobs;

public class EmailDeliveryProcessingJob
{
    private readonly ILogger<EmailDeliveryProcessingJob> _logger;
    private readonly IEmailsRepository _repo;
    private readonly IRecipientService _recipientService;
    private readonly SMTPSettings _config;

    public EmailDeliveryProcessingJob(
        ILogger<EmailDeliveryProcessingJob> logger,
        IOptions<SMTPSettings> config,
        IEmailsRepository repository,
        IRecipientService recipientService

        )
    {
        _logger = logger;
        _repo = repository;
        _recipientService = recipientService;
        _config = config.Value;
    }

    [AutomaticRetry(Attempts = 0)]
    [JobDisplayName("EmailDeliveryProcessingJob")]
    [Queue(HangfireQueues.DEFAULT)]
    public async Task Send(
        EmailNotification email,
        Recipient recipient,
        PerformContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("EmailDeliveryProcessingJob invoked");
        if (email == null)
        {
            throw new NullReferenceException("Email can't be null");
        }

        if (recipient == null)
        {
            _repo.ChangeEmailStatus(email.Id, EStatus.HasErrors);
            throw new NotFoundException("user not found");
        }

        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(_config.SenderName, _config.SenderEmail));
        mailMessage.To.Add(new MailboxAddress("Client", recipient.Email));
        mailMessage.Subject = email.Subject;
        mailMessage.Body = new TextPart("plain")
        {
            Text = email.Content
        };

        var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(_config.Host, _config.Port, _config.UseSSL, cancellationToken);
        }
        catch (Exception ex)
        {
            _repo.ChangeEmailStatus(email.Id, EStatus.HasErrors);
            _logger.LogError(ex.Message);
            return;
        }

        try
        {
            await smtpClient.AuthenticateAsync(_config.UserName, _config.Password, cancellationToken);
        }
        catch (Exception ex)
        {
            _repo.ChangeEmailStatus(email.Id, EStatus.HasErrors);
            _logger.LogError(ex.Message);
            return;
        }

        try
        {
            await smtpClient.SendAsync(mailMessage, cancellationToken);
            _repo.ChangeEmailStatus(email.Id, EStatus.Send);
            Console.WriteLine($"Email {email.Id} send successfully");
        }
        catch (Exception ex)
        {
            _repo.ChangeEmailStatus(email.Id, EStatus.HasErrors);
            _logger.LogError(ex.Message);
            return;
        }
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}