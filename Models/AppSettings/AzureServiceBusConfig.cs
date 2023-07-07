namespace NotificationService.Models.AppSettings
{
    public class AzureServiceBusConfig
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}