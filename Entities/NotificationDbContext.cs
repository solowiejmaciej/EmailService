using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Models.Responses;

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
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}