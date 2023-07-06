using Microsoft.AspNetCore.Identity;

namespace NotificationService.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? DeviceId { get; set; }
    }
}