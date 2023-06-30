using System.Text.RegularExpressions;
using FluentValidation;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;

namespace NotificationService.Models.Validation
{
    public class SmsRequestValidation : AbstractValidator<SmsRequest>
    {
        public SmsRequestValidation()
        {
            RuleFor(e => e.Body)
                .NotEmpty();
            RuleFor(e => e.To)
                .Length(9)
                .Matches(new Regex(@"^\+?[1-9][0-9]{8,8}$")).WithMessage("PhoneNumber not valid")
                .NotEmpty();
        }
    }
}