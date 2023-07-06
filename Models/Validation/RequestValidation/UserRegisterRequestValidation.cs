using System.Text.RegularExpressions;
using FluentValidation;
using NotificationService.Entities;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class UserRegisterRequestValidation : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidation(NotificationDbContext dbContext)
        {
            RuleFor(u => u.Email)
                .EmailAddress()
                .NotEmpty()
                .Custom(
                    (value, context) =>
                    {
                        var emailInUse = dbContext.Users.Any(u => u.Email == value);

                        if (emailInUse)
                        {
                            context.AddFailure("Email", "Already in use");
                        }
                    });
            RuleFor(u => u.Password)
                .NotEmpty()
                .Equal(u => u.ConfirmPassword);
            RuleFor(u => u.ConfirmPassword)
                .NotEmpty();
            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .Matches(new Regex(@"^\+?[1-9][0-9]{8,8}$")).WithMessage("PhoneNumber not valid");
        }
    }
}