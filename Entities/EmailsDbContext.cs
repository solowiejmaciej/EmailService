using Microsoft.EntityFrameworkCore;

namespace EmailService.Entities
{
    public class EmailsDbContext : DbContext
    {
        public EmailsDbContext(DbContextOptions<EmailsDbContext> options) : base(options)
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<User> Users { get; set; }
    }
}