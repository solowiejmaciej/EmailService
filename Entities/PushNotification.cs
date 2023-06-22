namespace NotificationService.Entities
{
    public enum PushStatus
    {
        New,
        Send,
        HasErrors
    }

    public class PushNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public PushStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}