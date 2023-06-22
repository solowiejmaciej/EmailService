using FluentValidation;

namespace NotificationService.Models.Validation
{
    public class EmailDtoValidation : AbstractValidator<EmailDto>
    {
        public EmailDtoValidation()
        {
            RuleFor(e => e.Body)
                .MinimumLength(8)
                .NotEmpty();
            RuleFor(e => e.Subject)
                .MinimumLength(4)
                .NotEmpty();
            RuleFor(e => e.EmailTo)
                .NotEmpty()
                .EmailAddress();
            RuleFor(e => e.EmailSenderName)
                .MinimumLength(2)
                .NotEmpty();
        }
    }
}