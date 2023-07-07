using FluentValidation;
using NotificationService.Entities;

namespace NotificationService.Models.Validation.Custom
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> IsValidUserId<T>(this IRuleBuilder<T, string> ruleBuilder, NotificationDbContext dbContext)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(value =>
                {
                    var isUserIdValid = dbContext.Users.Any(u => u.Id == value);
                    return isUserIdValid;
                })
                .WithMessage((value, property) => $"{property} is not a valid UserId");
        }
    }
}