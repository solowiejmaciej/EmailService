using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Models.Dtos;

namespace NotificationService.Repositories
{
    public interface IUserRepository : IDisposable
    {
        Task<string> AddAsyncWithDefaultRole(ApplicationUser user);

        Task<ApplicationUser?> GetByIdAsync(string id);

        Task<List<ApplicationUser>> GetAllAsync();

        Task SaveAsync();
    }

    public class UsersRepository : IUserRepository
    {
        private readonly NotificationDbContext _dbContext;

        public UsersRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddAsyncWithDefaultRole(ApplicationUser user)
        {
            var userInDb = await _dbContext.Users.AddAsync(user);
            var roleUserId = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            await _dbContext.UserRoles.AddAsync(new()
            {
                RoleId = roleUserId!.Id,
                UserId = userInDb.Entity.Id
            });
            await _dbContext.SaveChangesAsync();
            return userInDb.Entity.Id;
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<int> SoftDeleteAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public void SoftDelete(string id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}