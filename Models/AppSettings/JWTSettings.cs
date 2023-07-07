namespace NotificationService.Models.AppSettings
{
    public class JWTSettings
    {
        public string JwtPublicKey { get; set; }
        public int JwtExpireMinutes { get; set; }
        public string JwtIssuer { get; set; }
    }
}