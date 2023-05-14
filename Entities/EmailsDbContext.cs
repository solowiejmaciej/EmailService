using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Entities
{
    public class EmailsDbContext : IdentityDbContext
    {
        public EmailsDbContext(DbContextOptions<EmailsDbContext> options) : base(options)
        {
        }

        public DbSet<Email> Emails { get; set; }
    }
}