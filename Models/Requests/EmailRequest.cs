namespace NotificationService.Models.Requests
{
    public class EmailRequest
    {
        public string EmailSenderName { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string Body { get; set; }
    }
}