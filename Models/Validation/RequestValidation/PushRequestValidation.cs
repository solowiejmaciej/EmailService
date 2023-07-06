using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class PushRequestValidation : AbstractValidator<PushRequest>
    {
        public PushRequestValidation()
        {
            RuleFor(e => e.Content)
                .NotEmpty();
            RuleFor(e => e.Title)
                .NotEmpty();
        }
    }
}