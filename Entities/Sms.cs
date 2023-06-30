namespace NotificationService.Entities
{
    public class Sms
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedById { get; set; }
        public SmsStatus Status { get; set; }
    }

    public enum SmsStatus
    {
        New,
        Send,
        HasErrors
    }
}