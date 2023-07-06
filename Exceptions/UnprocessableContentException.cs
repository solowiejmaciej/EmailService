namespace NotificationService.Exceptions
{
    public class UnprocessableContentException : Exception
    {
        public UnprocessableContentException(string message) : base(message)
        {
        }
    }
}