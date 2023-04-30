namespace EmailService.Entities
{
    public class Email
    {
        public int Id { get; set; }
        public string? EmailSenderName { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string Body { get; set; }
        public bool isEmailSended { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedById { get; set; }
        public bool? IsDeleted { get; set; }
    }
}