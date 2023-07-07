using Microsoft.AspNetCore.Identity;

namespace NotificationService.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? DeviceId { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}