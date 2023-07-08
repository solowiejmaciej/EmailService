using NotificationService.Entities.NotificationEntities;
using NotificationService.Services;

namespace NotificationService.Repositories.Cached;

public class CachedSmsRepository : ISmsRepository
{
    private readonly ISmsRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedSmsRepository(ISmsRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }
    
    public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var key = $"sms-{id}";
        var secondDey = $"sms-{userId}";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
        await _cacheService.RemoveDataAsync(secondDey, cancellationToken);
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
        await _decorated.AddAsync(sms, cancellationToken);
    }
    

    public async Task<List<SmsNotification>> GetAllSmsToUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var key = $"sms-es-{userId}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<List<SmsNotification>>(key, cancellationToken);
        if (cachedData is null)
        {
            var data = await _decorated.GetAllSmsToUserIdAsync(userId, cancellationToken);
            if (data is null)
            {
                return data;
            }

            await _cacheService.SetDataAsync(key, data ,expiryTime, cancellationToken );
            return data;
        }
        return cachedData;
    }

    public async Task<SmsNotification?> GetSmsByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        
        string key = $"sms-{id}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<SmsNotification>(key, cancellationToken);
        if (cachedData is null)
        {
            var data = await _decorated.GetSmsByIdAndUserIdAsync(id, userId, cancellationToken);

            if (data is null)
            {
                return data;
            }

            await _cacheService.SetDataAsync(key, data ,expiryTime, cancellationToken );
            return data;
        }
        
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