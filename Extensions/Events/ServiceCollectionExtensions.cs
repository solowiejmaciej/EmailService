using NotificationService.Models.AppSettings;
using Microsoft.Azure.ServiceBus;
using NotificationService.Events;

namespace NotificationService.Extensions.Events
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAzureServiceBus(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var azureServiceBusConfig = new AzureServiceBusSettings();
            var azureConfigurationSection = configuration.GetSection("AzureServiceBusSettings");
            azureConfigurationSection.Bind(azureServiceBusConfig);

            services.AddSingleton<IQueueClient>(
                x =>
                    new QueueClient(azureServiceBusConfig.ConnectionString, azureServiceBusConfig.QueueName)
                );

            services.AddScoped<IEventsPublisher, EventsPublisher>();
        }
    }
}