using EmailService.Entities;
using EmailService.Models;

namespace EmailService.Services;

public interface IEmailSenderService
{
    private async Task Send(Email email)
    {
    }

    Task SendEmailNow(EmailDto email);

    Task<int> AddNewEmailToDbAsync(EmailDto dto);

    Task AddTestEmail();

    Task<Task> SendInBackground();
}