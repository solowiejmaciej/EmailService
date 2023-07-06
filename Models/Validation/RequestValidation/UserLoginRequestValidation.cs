using FluentValidation;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class UserLoginRequestValidation : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidation()
        {
            RuleFor(u => u.Email)
                .NotEmpty();
            RuleFor(u => u.Password)
                .NotEmpty();
        }
    }
}