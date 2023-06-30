using NotificationService.Entities;

namespace NotificationService.Models.Dtos
{
    public class SmsDto
    {
        public string Body { get; set; }
        public string To { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedById { get; set; }
        public SmsStatus Status { get; set; }
    }
}