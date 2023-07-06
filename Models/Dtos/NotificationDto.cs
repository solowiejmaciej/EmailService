using NotificationService.Entities.NotificationEntities;

namespace NotificationService.Models.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RecipientId { get; set; }
        public EStatus Status { get; set; }
        public string Content { get; set; }
    }
}