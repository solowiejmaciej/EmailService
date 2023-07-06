using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public class DeletePushCommand : IRequest
    {
        public int Id { get; set; }
    }
}