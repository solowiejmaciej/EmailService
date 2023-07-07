namespace NotificationService.Exceptions
{
    public class AccessForbiddenException : Exception
    {
        public AccessForbiddenException(string message) : base(message)
        {
        }
    }
}