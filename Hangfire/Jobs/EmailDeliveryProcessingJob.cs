using Hangfire;
using Hangfire.Server;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Entities;
using NotificationService.Models.AppSettings;
using NotificationService.Services;

namespace NotificationService.Hangfire.Jobs;

public class EmailDeliveryProcessingJob
{
    private readonly ILogger<EmailDeliveryProcessingJob> _logger;
    private readonly IEmailDataService _emailService;
    private readonly SMTPConfig _config;

    public EmailDeliveryProcessingJob(ILogger<EmailDeliveryProcessingJob> logger, IOptions<SMTPConfig> config, IEmailDataService emailService)
    {
        _logger = logger;
        _emailService = emailService;
        _config = config.Value;
    }

    [AutomaticRetry(Attempts = 0)]
    [JobDisplayName("EmailDeliveryProcessingJob")]
    [Queue(HangfireQueues.DEFAULT)]
    public async Task Send(
        Email email,
        PerformContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("EmailDeliveryProcessingJob invoked");
        if (email == null)
        {
            throw new NullReferenceException("Email can't be null");
        }
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(email.EmailSenderName, email.EmailFrom));
        mailMessage.To.Add(new MailboxAddress("Client", email.EmailTo));
        mailMessage.Subject = email.Subject;
        mailMessage.Body = new TextPart("plain")
        {
            Text = email.Body
        };

        var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(_config.Host, _config.Port, _config.UseSSL, cancellationToken);
        }
        catch (Exception ex)
        {
            _emailService.ChangeEmailStatus(email.Id, EmailStatus.HasErrors);
            _logger.LogError(ex.Message);
            return;
        }

        try
        {
            await smtpClient.AuthenticateAsync(_config.UserName, _config.Password, cancellationToken);
        }
        catch (Exception ex)
        {
            _emailService.ChangeEmailStatus(email.Id, EmailStatus.HasErrors);
            _logger.LogError(ex.Message);
            return;
        }

        try
        {
            await smtpClient.SendAsync(mailMessage, cancellationToken);
            _emailService.ChangeEmailStatus(email.Id, EmailStatus.EmailSended);
            Console.WriteLine($"Email {email.Id} send successfully");
        }
        catch (Exception ex)
        {
            _emailService.ChangeEmailStatus(email.Id, EmailStatus.HasErrors);
            _logger.LogError(ex.Message);
            return;
        }
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}