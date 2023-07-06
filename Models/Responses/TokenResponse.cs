namespace NotificationService.Models.Responses
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public int StatusCode { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
    }
}