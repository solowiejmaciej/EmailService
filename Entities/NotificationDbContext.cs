using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Entities
{
    public class NotificationDbContext : IdentityDbContext<ApplicationUser>
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<EmailNotification> EmailsNotifications { get; set; }
        public DbSet<PushNotification> PushNotifications { get; set; }
        public DbSet<SmsNotification> SmsNotifications { get; set; }
    }
}