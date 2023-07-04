using Hangfire;
using NotificationService.Hangfire.Manager;
using NotificationService.Repositories;
using HangfireBasicAuthenticationFilter;

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

        services.AddScoped<INotificationJobManager, NotificationJobManager>();

        return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        var notificationAppSettings = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var hangfireConfig = notificationAppSettings.GetSection("HangfireSettings");

        app.UseHangfireDashboard("/hangfire", new DashboardOptions()
        {
            DashboardTitle = "NotificationService",
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter()
                {
                    User = hangfireConfig["UserName"],
                    Pass = hangfireConfig["Password"]
                }
            }
        });
        RecurringJob.AddOrUpdate<IEmailsRepository>(
            "DeleteAllEmails",
            x => x.DeleteInBackground(),
            Cron.Minutely,
            queue: HangfireQueues.LOW_PRIORITY
            );
        RecurringJob.AddOrUpdate<ISmsRepository>(
            "DeleteSMS",
            x => x.DeleteInBackground(),
            Cron.Minutely,
            queue: HangfireQueues.LOW_PRIORITY
        );
        RecurringJob.AddOrUpdate<IPushRepository>(
            "DeletePush",
            x => x.DeleteInBackground(),
            Cron.Minutely,
            queue: HangfireQueues.LOW_PRIORITY
        );

        return app;
    }
}