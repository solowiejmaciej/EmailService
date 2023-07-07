using Hangfire;
using HangfireBasicAuthenticationFilter;
using NotificationService.Hangfire;
using NotificationService.Hangfire.Manager;
using NotificationService.Repositories;

namespace NotificationService.Extensions.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireServiceCollection(this IServiceCollection services)
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
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var hangfireSettings = configuration.GetSection("HangfireSettings");

        app.UseHangfireDashboard("/hangfire", new DashboardOptions()
        {
            DashboardTitle = "NotificationService",
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter()
                {
                    User = hangfireSettings["UserName"],
                    Pass = hangfireSettings["Password"]
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