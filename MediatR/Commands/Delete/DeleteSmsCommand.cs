using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public record DeleteSmsCommand : IRequest
    {
        public int Id { get; set; }
    }
}