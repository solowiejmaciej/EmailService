using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public class DeleteEmailCommand : IRequest
    {
        public int Id { get; set; }
    }
}