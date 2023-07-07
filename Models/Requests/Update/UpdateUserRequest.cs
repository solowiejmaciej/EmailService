namespace NotificationService.Models.Requests.Update
{
    public class UpdateUserRequest
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeviceId { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}