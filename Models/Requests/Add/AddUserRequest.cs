namespace NotificationService.Models.Requests
{
    public class AddUserRequest
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? DeviceId { get; set; }
    }
}