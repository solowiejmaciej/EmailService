using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public class DeleteUserCommand : IRequest
    {
        public string Id { get; set; }
    }
}