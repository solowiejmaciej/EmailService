namespace NotificationService.Entities.NotificationEntities
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string RecipientId { get; set; }
        public EStatus Status { get; set; }
        public string Content { get; set; }
    }

    public enum EStatus
    {
        New,
        Send,
        HasErrors,
        ToBeDeleted
    }
}