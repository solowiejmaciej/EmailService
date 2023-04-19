namespace EmailService.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public int StatusCode { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; }
    }
}
