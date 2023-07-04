namespace NotificationService.Middleware
{
    public class ApiKeyAuthMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthMiddleware(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing");
                return;
            }

            var apiKey = _configuration.GetValue<string>("Auth:ApiKey");
            if (!apiKey!.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid ApiKey");
                return;
            }

            await next(context);
        }
    }
}