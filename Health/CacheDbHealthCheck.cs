using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Services;

namespace NotificationService.Health
{
    public class CacheDbHealthCheck : IHealthCheck
    {
        private readonly ICacheService _cacheService;

        public CacheDbHealthCheck(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _cacheService.SetDataAsync("Health", "I'm healthy!", DateTimeOffset.Now.AddMinutes(2));
                await _cacheService.GetDataAsync<string>("Health");
            }
            catch (Exception e)
            {
                return HealthCheckResult.Degraded(e.Message);
            }

            return HealthCheckResult.Healthy();
        }
    }
}