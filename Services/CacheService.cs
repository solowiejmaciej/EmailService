using System.Text.Json;
using EmailService.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace EmailService.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);

        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

        object RemoveData(string key);
    }

    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;

        public CacheService(IOptions<RedisSettings> config)

        {
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { config.Value.Endpoints },
                Password = config.Value.Password
            }
            );
            _cacheDb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
            return isSet;
        }

        public object RemoveData(string key)
        {
            var exists = _cacheDb.KeyExists(key);
            if (exists)
            {
                return _cacheDb.KeyDelete(key);
            }

            return false;
        }
    }
}