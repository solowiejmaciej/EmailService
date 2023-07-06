using FluentValidation;
using NotificationService.Models.Validation;

namespace NotificationService.Models.Requests
{
    public class UserRegisterRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? DeviceId { get; set; }
    }
}