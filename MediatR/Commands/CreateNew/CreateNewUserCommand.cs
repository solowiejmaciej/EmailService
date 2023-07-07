using MediatR;
using NotificationService.Models.Responses;

namespace NotificationService.MediatR.Commands.CreateNew
{
    public record CreateNewUserCommand : IRequest<TokenResponse>
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? DeviceId { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}