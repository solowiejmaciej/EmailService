using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Entities
{
    public class NotificationDbContext : IdentityDbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<Email> Emails { get; set; }
        public DbSet<PushNotification> PushNotifications { get; set; }
    }
}