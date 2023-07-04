namespace NotificationService.Entities
{
    public enum EmailStatus
    {
        New,
        Send,
        HasErrors,
        ToBeDeleted
    }

    public class Email
    {
        public int Id { get; set; }
        public string? EmailSenderName { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public EmailStatus EmailStatus { get; set; }
        public string? CreatedById { get; set; }
    }
}