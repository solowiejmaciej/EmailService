using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NotificationService.Models.AppSettings;
using RestSharp;

namespace NotificationService.Health
{
    internal class BalanceResponse
    {
        public int balance { get; set; }
    }

    public class SmsPlanetApiHealthCheck : IHealthCheck
    {
        private readonly SmsSettings _config;

        public SmsPlanetApiHealthCheck(IOptions<SmsSettings> config)
        {
            _config = config.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var baseUrl = _config.ApiUrl;

            var options = new RestClientOptions(baseUrl);
            var client = new RestClient(options);
            var request = new RestRequest("/getBalance", Method.Post);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("key", _config.Key);
            request.AddParameter("password", _config.Password);

            var response = await client.ExecuteAsync<BalanceResponse>(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Degraded();
            }

            if (response.Data == null)
            {
                return HealthCheckResult.Degraded();
            }

            return HealthCheckResult.Healthy();
        }
    }
}