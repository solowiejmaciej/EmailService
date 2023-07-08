using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;

namespace NotificationService.Repositories
{
    public interface IUsersRepository : IDisposable
    {
        Task<string> AddAsyncWithDefaultRole(ApplicationUser user, CancellationToken cancellationToken = default);

        Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<List<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default);

        Task SaveAsync(CancellationToken cancellationToken = default);
    }

    public sealed class UsersRepository : IUsersRepository
    {
        private readonly NotificationDbContext _dbContext;

        public UsersRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddAsyncWithDefaultRole(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var userInDb = await _dbContext.Users.AddAsync(user, cancellationToken);
            var roleUserId = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);
            await _dbContext.UserRoles.AddAsync(new IdentityUserRole<string>
            {
                RoleId = roleUserId!.Id,
                UserId = userInDb.Entity.Id,
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return userInDb.Entity.Id;
        }

        public async Task<List<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.ToListAsync(cancellationToken);
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public Task<int> SoftDeleteAsync(string userId)
        {
            throw new NotImplementedException();
        }
        
        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private bool _disposed;

        private void Dispose(bool disposing)
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