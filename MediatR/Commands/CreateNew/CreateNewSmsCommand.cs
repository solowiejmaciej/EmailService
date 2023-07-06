using MediatR;

namespace NotificationService.MediatR.Commands.CreateNew
{
    public class CreateNewSmsCommand : IRequest<int>
    {
        public string Content { get; set; }
        public string RecipiantId { get; set; }
    }
}