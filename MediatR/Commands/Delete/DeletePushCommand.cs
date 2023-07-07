using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public record DeletePushCommand : IRequest
    {
        public int Id { get; set; }
    }
}