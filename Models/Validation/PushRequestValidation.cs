using FluentValidation;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation
{
    public class PushRequestValidation : AbstractValidator<PushRequest>
    {
        public PushRequestValidation()
        {
            RuleFor(e => e.PushContent)
                .NotEmpty();
            RuleFor(e => e.PushTitle)
                .NotEmpty();
        }
    }
}