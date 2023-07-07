using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public record DeleteUserCommand : IRequest
    {
        public string Id { get; set; }
    }
}