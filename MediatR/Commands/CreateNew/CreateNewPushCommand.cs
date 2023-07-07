using MediatR;

namespace NotificationService.MediatR.Commands.CreateNew
{
    public record CreateNewPushCommand : IRequest<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string RecipiantId { get; set; }
    }
}