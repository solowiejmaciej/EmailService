using System.Text.RegularExpressions;
using FluentValidation;
using NotificationService.Entities;
using NotificationService.Models.Requests;
using NotificationService.Models.Requests.Update;

namespace NotificationService.Models.Validation.RequestValidation
{
    public class UpdateUserRequestValidation : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidation(NotificationDbContext dbContext)
        {
            RuleFor(u => u.Email)
                .EmailAddress()
                .Custom(
                    (value, context) =>
                    {
                        var emailInUse = dbContext.Users.Any(u => u.Email == value);

                        if (emailInUse)
                        {
                            context.AddFailure("Email", "Already in use");
                        }
                    });
            RuleFor(u => u.PhoneNumber)
                .Matches(new Regex(@"^\+?[1-9][0-9]{8,8}$")).WithMessage("PhoneNumber not valid");
            RuleFor(u => u.Firstname)
                .MinimumLength(2)
                .MaximumLength(25);
            RuleFor(u => u.Surname)
                .MinimumLength(2)
                .MaximumLength(25);
            RuleFor(u => u.DeviceId)
                .MinimumLength(10)
                .MaximumLength(100);
        }
    }
}