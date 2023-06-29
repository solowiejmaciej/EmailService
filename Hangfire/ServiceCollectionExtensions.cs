using System.Reflection;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace NotificationService.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHangfire(this IServiceCollection services)
    {

        var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

        services.AddHangfire(config => config
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("Hangfire")));

        services.AddHangfireServer((serviceProvider, bjsOptions) =>
        {
            bjsOptions.ServerName = "NotificationServiceServer";
            bjsOptions.Queues = new[]
            {
                HangfireQueues.HIGH_PRIORITY,
                HangfireQueues.MEDIUM_PRIORITY,
                HangfireQueues.LOW_PRIORITY,
                HangfireQueues.DEFAULT
            };
        });
        return services;
    }
}