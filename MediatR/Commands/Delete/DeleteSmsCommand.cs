using MediatR;

namespace NotificationService.MediatR.Commands.Delete
{
    public class DeleteSmsCommand : IRequest
    {
        public int Id { get; set; }
    }
}