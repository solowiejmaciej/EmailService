using Hangfire;
using MediatR;
using NotificationService.MediatR.Commands.Delete;

namespace NotificationService.MediatR.Handlers.Delete
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        public Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}