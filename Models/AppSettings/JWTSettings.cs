namespace NotificationService.Models.AppSettings
{
    public class JWTSettings
    {
        public string PublicKey { get; set; }
        public int ExpireMinutes { get; set; }
        public string Issuer { get; set; }
    }
}