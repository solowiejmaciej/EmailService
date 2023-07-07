using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NotificationService.Models.AppSettings;

namespace NotificationService.Middleware
{
    public class ApiKeyAuthMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<ApiKeySettings> _config;

        public ApiKeyAuthMiddleware(
            IConfiguration configuration,
            IOptions<ApiKeySettings> config
        )
        {
            _configuration = configuration;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue(_config.Value.HeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing");
                return;
            }

            var apiKey = _config.Value.ApiKey;
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid ApiKey");
                return;
            }

            await next(context);
        }
    }
}