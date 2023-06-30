namespace NotificationService.Models.Requests
{
    public class SmsRequest
    {
        public string Body { get; set; }
        public string To { get; set; }
    }
}