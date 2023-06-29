namespace NotificationService.Hangfire;

public static class HangfireQueues
{
    public const string LOW_PRIORITY = "3-low-priority";
    public const string MEDIUM_PRIORITY = "2-medium-priority";
    public const string HIGH_PRIORITY = "1-high-priority";
    public const string DEFAULT = "default";
}