using NotificationService.Entities.NotificationEntities;
using NotificationService.Services;

namespace NotificationService.Repositories.Cached;

public class CachedSmsRepository : ISmsRepository
{
    private readonly ISmsRepository _decorated;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedSmsRepository> _logger;


    public CachedSmsRepository(ISmsRepository decorated, ICacheService cacheService, ILogger<CachedSmsRepository> logger)
    {
        _decorated = decorated;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var key = $"sms-{id}";
        var secondKey = $"sms-es-{userId}";
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
    
    public async Task AddAsync(SmsNotification sms, CancellationToken cancellationToken = default)
    {
        var key = $"sms-es-{sms.RecipientId}";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
        _logger.LogInformation("cached key {0} removed", key);
        await _decorated.AddAsync(sms, cancellationToken);
    }
    

    public async Task<List<SmsNotification>> GetAllSmsToUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var key = $"sms-es-{userId}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<List<SmsNotification>>(key, cancellationToken);
        if (cachedData is null)
        {
            _logger.LogInformation("Fetching from db for key {0}", key);
            var data = await _decorated.GetAllSmsToUserIdAsync(userId, cancellationToken);
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

    public async Task<SmsNotification?> GetSmsByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        
        string key = $"sms-{id}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<SmsNotification>(key, cancellationToken);
        if (cachedData is null)
        {
            _logger.LogInformation("Fetching from db for key {0}", key);

            var data = await _decorated.GetSmsByIdAndUserIdAsync(id, userId, cancellationToken);

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

    public void ChangeSmsStatus(int id, EStatus status)
    {
        var key = $"sms-{id}";

        _cacheService.RemoveDataAsync(key);
        _decorated.ChangeSmsStatus(id, status);
    }
    
    public void Dispose()
    {
        _decorated.Dispose();
    }

    public void DeleteInBackground()
    {
        _decorated.DeleteInBackground();
    }
}