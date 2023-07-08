using NotificationService.Entities.NotificationEntities;
using NotificationService.Services;

namespace NotificationService.Repositories.Cached;

public class CachedEmailsRepository : IEmailsRepository
{
    private readonly IEmailsRepository _decorated;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedEmailsRepository> _logger;

    public CachedEmailsRepository(IEmailsRepository decorated, ICacheService cacheService, ILogger<CachedEmailsRepository> logger)
    {
        _decorated = decorated;
        _cacheService = cacheService;
        _logger = logger;
    }


    public async Task<int> SoftDeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var key = $"email-{id}";
        var secondKey = $"email-{userId}";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
        await _cacheService.RemoveDataAsync(secondKey, cancellationToken);
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

    public void Dispose()
    {
        _decorated.Dispose();
    }

    public async Task<int> AddAsync(EmailNotification email, CancellationToken cancellationToken = default)
    {
        var key = $"emails-{email.RecipientId}";
        await _cacheService.RemoveDataAsync(key, cancellationToken);
        _logger.LogInformation("cached key {0} removed", key);
        return await _decorated.AddAsync(email, cancellationToken);
    }

    public async Task<List<EmailNotification>> GetAllEmailsToUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var key = $"emails-{userId}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<List<EmailNotification>>(key, cancellationToken);
        if (cachedData is null)
        {
            _logger.LogInformation("Fetching from db for key {0}", key);
            var data = await _decorated.GetAllEmailsToUserIdAsync(userId, cancellationToken);
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

    public async Task<EmailNotification?> GetEmailByIdAndUserIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        
        string key = $"email-{id}";
        var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        var cachedData = await _cacheService.GetDataAsync<EmailNotification>(key, cancellationToken);
        if (cachedData is null)
        {
            _logger.LogInformation("Fetching from db for key {0}", key);
            var data = await _decorated.GetEmailByIdAndUserIdAsync(id, userId, cancellationToken);

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

    public void ChangeEmailStatus(int id, EStatus status)
    {
        var key = $"email-{id}";
        _cacheService.RemoveDataAsync(key);
        _decorated.ChangeEmailStatus(id, status);
    }
}