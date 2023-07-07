using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class AddSmsRequestValidation : AbstractValidator<AddSmsRequest>
    {
        public AddSmsRequestValidation()
        {
            RuleFor(e => e.Content)
                .NotEmpty();
        }
    }
}