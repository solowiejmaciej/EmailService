using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class AddPushRequestValidation : AbstractValidator<AddPushRequest>
    {
        public AddPushRequestValidation()
        {
            RuleFor(e => e.Content)
                .NotEmpty();
            RuleFor(e => e.Title)
                .NotEmpty();
        }
    }
}