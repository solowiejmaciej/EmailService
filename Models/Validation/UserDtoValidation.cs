using FluentValidation;

namespace EmailService.Models.Validation
{
    public class UserDtoValidation : AbstractValidator<UserDto>
    {
        public UserDtoValidation()
        {
            RuleFor(u => u.Login)
                .NotEmpty();
            RuleFor(u => u.Password)
                .NotEmpty();
        }
    }
}