using Microsoft.Extensions.Options;
using NotificationService.Models.AppSettings;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NotificationService.Services
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default);

        Task SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime, CancellationToken cancellationToken = default);

        Task RemoveDataAsync(string key, CancellationToken cancellationToken = default);
    }

    public class CacheService : ICacheService
    {
        private IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var value = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public async Task SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime, CancellationToken cancellationToken = default)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(value),
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = expiryTime
                },
                cancellationToken
            );
        }
        public async Task RemoveDataAsync (string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}