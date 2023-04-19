namespace EmailService.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get ; set; }
        public string PasswordHashed { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    }
}
