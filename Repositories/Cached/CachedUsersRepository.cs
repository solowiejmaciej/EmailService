using NotificationService.Entities;
using NotificationService.Services;

namespace NotificationService.Repositories.Cached;

public class CachedUsersRepository : IUsersRepository
{
    private readonly IUsersRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedUsersRepository(IUsersRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<string> AddAsyncWithDefaultRole(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var key = "users";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
       return await _decorated.AddAsyncWithDefaultRole(user, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        string key = $"user-{id}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedUser = await _cacheService.GetDataAsync<ApplicationUser>(key, cancellationToken);
        if (cachedUser is null)
        {
            var user = await _decorated.GetByIdAsync(id, cancellationToken);
            if (user is null)
            {
                return user;
            }

            await _cacheService.SetDataAsync(key, user ,expiryTime, cancellationToken );
            return user;
        }
        
        return cachedUser;
    }
    
    public async Task<List<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        string key = "users";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedUsers = await _cacheService.GetDataAsync<List<ApplicationUser>>(key, cancellationToken);
        if (cachedUsers is null)
        {
            var users = await _decorated.GetAllAsync(cancellationToken);
            if (users is null)
            {
                return users;
            }

            await _cacheService.SetDataAsync(key, users ,expiryTime, cancellationToken );
            return users;
        }
        return cachedUsers;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _decorated.SaveAsync(cancellationToken);
    }

    public void Dispose()
    {
        _decorated.Dispose();
    }
}