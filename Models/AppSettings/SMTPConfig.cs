namespace EmailService
{
    public class SMTPConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailFrom { get; set; }
    }
}