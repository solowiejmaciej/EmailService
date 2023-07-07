using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class AddEmailRequestValidation : AbstractValidator<AddEmailRequest>
    {
        public AddEmailRequestValidation()
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