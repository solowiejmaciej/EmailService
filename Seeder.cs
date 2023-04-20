using EmailService.Entities;
using EmailService.Models;
using EmailService.Services;
using Microsoft.EntityFrameworkCore;

namespace EmailService
{
    public class Seeder
    {
        private readonly EmailsDbContext _dbContext;
        private readonly IAuthService _authService;

        public Seeder(EmailsDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }

                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                //To refactor while adding UserService
                if (!_dbContext.Users.Any())
                {
                    var User = new UserDto()
                    {
                        Login = "cwsuser",
                        Password = "string"
                    };
                    _authService.AddNewUser(User);
                    var createdUser = _dbContext.Users.FirstOrDefault(u => u.Login == User.Login);
                    createdUser.RoleId = 2;
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new ()
                {
                    Name = "User"
                },
                new ()
                {
                    Name = "Admin"
                }
            };

            return roles;
        }
    }
}