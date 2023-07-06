using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class EmailRequestValidation : AbstractValidator<EmailRequest>
    {
        public EmailRequestValidation()
        {
            RuleFor(e => e.Content)
                .MinimumLength(8)
                .NotEmpty();
            RuleFor(e => e.Subject)
                .MinimumLength(4)
                .NotEmpty();
        }
    }
}