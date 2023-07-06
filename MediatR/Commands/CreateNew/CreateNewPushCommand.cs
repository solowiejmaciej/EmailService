using MediatR;

namespace NotificationService.MediatR.Commands.CreateNew
{
    public class CreateNewPushCommand : IRequest<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string RecipiantId { get; set; }
    }
}