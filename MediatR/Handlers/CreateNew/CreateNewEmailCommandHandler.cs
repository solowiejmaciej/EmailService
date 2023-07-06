using AutoMapper;
using MediatR;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Exceptions;
using NotificationService.Hangfire.Manager;
using NotificationService.MediatR.Commands;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.Repositories;
using NotificationService.Services;

namespace NotificationService.MediatR.Handlers.CreateNew
{
    public class CreateNewEmailCommandHandler : IRequestHandler<CreateNewEmailCommand, int>
    {
        private readonly IMapper _mapper;
        private readonly IEmailsRepository _repository;
        private readonly INotificationJobManager _jobManager;
        private readonly IRecipientService _recipientService;

        public CreateNewEmailCommandHandler(
            IMapper mapper,
            IEmailsRepository repository,
            INotificationJobManager jobManager,
            IRecipientService recipientService
        )
        {
            _mapper = mapper;
            _repository = repository;
            _jobManager = jobManager;
            _recipientService = recipientService;
        }

        public async Task<int> Handle(CreateNewEmailCommand request, CancellationToken cancellationToken)
        {
            var email = _mapper.Map<EmailNotification>(request);

            var recipient = _recipientService.GetRecipientFromUserId(request.RecipiantId);

            if (recipient is null)
            {
                throw new NotFoundException("Recipient not found");
            }

            email.RecipientId = request.RecipiantId;
            await _repository.AddAsync(email);
            await _repository.SaveAsync();
            _jobManager.EnqueueEmailDeliveryDeliveryJob(email, recipient);
            return email.Id;
        }
    }
}