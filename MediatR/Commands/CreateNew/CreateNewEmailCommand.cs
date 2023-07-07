using MediatR;

namespace NotificationService.MediatR.Commands.CreateNew
{
    public record CreateNewEmailCommand : IRequest<int>
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string RecipiantId { get; set; }
    }
}