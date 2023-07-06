using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class SmsRequestValidation : AbstractValidator<SmsRequest>
    {
        public SmsRequestValidation()
        {
            RuleFor(e => e.Content)
                .NotEmpty();
        }
    }
}