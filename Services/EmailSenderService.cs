using System.Security.Claims;
using AuthService;
using AutoMapper;
using EmailService.Entities;
using EmailService.Exceptions;
using EmailService.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Services;

public interface IEmailSenderService
{
    private async Task Send(Email email)
    {
    }

    Task SendEmailNow(EmailDto email);

    Task AddTestEmail();

    Task<Task> SendInBackground();
}

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailsDbContext _dbContext;
    private readonly ILogger<EmailSenderService> _logger;
    private readonly IEmailDataService _emailDataService;
    private readonly SMTPConfig _config;

    public EmailSenderService(EmailsDbContext dbContext, ILogger<EmailSenderService> logger, IOptions<SMTPConfig> config, IMapper mapper, IEmailDataService emailDataService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _emailDataService = emailDataService;
        _config = config.Value;
    }

    public async Task AddTestEmail()
    {
        var exampleMail = new Email()
        {
            EmailFrom = _config.EmailFrom,
            Body = "This is email send by invoking job",
            EmailTo = "maciejsol1926@gmail.com",
            Subject = "Test email from invoking hangfire job",
            EmailSenderName = "Test email",
            CreatedById = 0,
        };
        await _dbContext.AddAsync(exampleMail);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Test email added");
    }

    public async Task<Task> SendInBackground()
    {
        _logger.LogInformation($"{DateTime.UtcNow} || EmailService invoked");
        var emailsToSend = _dbContext.Emails.Where(e => e.isEmailSended == false).OrderBy(e => e.CreatedAt).Take(5).ToList();

        if (!emailsToSend.Any())
        {
            _logger.LogInformation("No emails to send found");
            return Task.CompletedTask;
        }

        _logger.LogInformation($"Going to send {emailsToSend.Count} emails");

        foreach (var email in emailsToSend)
        {
            await Send(email.Id);
        }

        return Task.CompletedTask;
    }

    private async Task Send(int id)
    {
        var email = _dbContext.Emails.FirstOrDefault(e => e.Id == id);
        if (email == null)
        {
            throw new NotFoundException($"Email id {id} not found");
        }
        var _mailMessage = new MimeMessage();
        _mailMessage.From.Add(new MailboxAddress(email.EmailSenderName, email.EmailFrom));
        _mailMessage.To.Add(new MailboxAddress("Client", email.EmailTo));
        _mailMessage.Subject = email.Subject;
        _mailMessage.Body = new TextPart("plain")
        {
            Text = email.Body
        };

        using (var smtpClient = new SmtpClient())
        {
            smtpClient.Connect(_config.Host, _config.Port, _config.UseSSL);
            smtpClient.Authenticate(_config.UserName, _config.Password);
            smtpClient.Send(_mailMessage);
            smtpClient.Disconnect(true);
            Console.WriteLine($"Email {email.Id} send succesfully");
            email.isEmailSended = true;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task SendEmailNow(EmailDto dto)
    {
        var id = await _emailDataService.AddNewEmailToDbAsync(dto);
        await Send(id);
    }
}