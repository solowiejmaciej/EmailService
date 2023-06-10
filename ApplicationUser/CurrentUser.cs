namespace EmailService.ApplicationUser
{
    public class CurrentUser
    {
        public CurrentUser(string id, string login, string role)
        {
            Id = id;
            Login = login;
            Role = role;
        }

        public string Id { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
    }
}