using NotificationService.Entities.NotificationEntities;
using NotificationService.Services;

namespace NotificationService.Repositories.Cached;

public class CachedPushRepository : IPushRepository
{
    private readonly IPushRepository _decorated;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedPushRepository> _logger;

    public CachedPushRepository(IPushRepository decorated, ICacheService cacheService, ILogger<CachedPushRepository> logger)
    {
        _decorated = decorated;
        _cacheService = cacheService;
        _logger = logger;
    }

    public void Dispose()
    {
        _decorated.Dispose();
    }

    public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var key = $"push-{id}";
        var secondKey = $"pushs-{userId}";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
        await _cacheService.RemoveDataAsync(secondKey, cancellationToken);
        _logger.LogInformation("cached key {0} removed", key);
        _logger.LogInformation("cached key {0} removed", secondKey);
        return await _decorated.SoftDeleteAsync(id, userId, cancellationToken);
    }
    
    public void Save()
    {
        _decorated.Save();
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _decorated.SaveAsync(cancellationToken);
    }

    public void DeleteInBackground()
    {
        _decorated.DeleteInBackground();
    }

    public async Task<int> AddAsync(PushNotification push, CancellationToken cancellationToken = default)
    {
        var key = $"pushs-{push.RecipientId}";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
        _logger.LogInformation("cached key {0} removed", key);
        return await _decorated.AddAsync(push, cancellationToken);
    }

    public async Task<List<PushNotification>> GetAllPushesToUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var key = $"pushs-{userId}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<List<PushNotification>>(key, cancellationToken);
        if (cachedData is null)
        {
            _logger.LogInformation("Fetching from db for key {0}", key);
            var data = await _decorated.GetAllPushesToUserIdAsync(userId, cancellationToken);
            if (data is null)
            {
                return data;
            }

            await _cacheService.SetDataAsync(key, data ,expiryTime, cancellationToken );
            return data;
        }
        _logger.LogInformation("Cache hit for key {0}", key);
        return cachedData;
    }

    public async Task<PushNotification?> GetPushByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        
        string key = $"push-{id}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<PushNotification>(key, cancellationToken);
        if (cachedData is null)
        {
            _logger.LogInformation("Fetching from db for key {0}", key);
            var data = await _decorated.GetPushByIdAndUserIdAsync(id, userId, cancellationToken);

            if (data is null)
            {
                return data;
            }

            await _cacheService.SetDataAsync(key, data ,expiryTime, cancellationToken );
            return data;
        }
        _logger.LogInformation("Cache hit for key {0}", key);
        return cachedData;
    }

    public void ChangePushStatus(int id, EStatus status)
    {
        var key = $"push-{id}";
        _cacheService.RemoveDataAsync(key);
        _decorated.ChangePushStatus(id, status);
    }
}