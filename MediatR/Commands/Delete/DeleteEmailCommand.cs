using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public record DeleteEmailCommand : IRequest
    {
        public int Id { get; set; }
    }
}