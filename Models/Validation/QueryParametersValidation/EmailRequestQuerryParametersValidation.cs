using FluentValidation;
using NotificationService.Entities;
using NotificationService.Models.QueryParameters;
using NotificationService.Models.QueryParameters.Create;
using NotificationService.Models.Validation.Custom;

namespace NotificationService.Models.Validation.QueryParametersValidation
{
    public class EmailRequestQuerryParametersValidation : AbstractValidator<CreateEmailRequestQueryParameters>
    {
        public EmailRequestQuerryParametersValidation(NotificationDbContext dbContext)
        {
            RuleFor(e => e.UserId)
                .NotEmpty()!
                .IsValidUserId(dbContext);
        }
    }
}