using AutoMapper;
using MediatR;
using NotificationService.Exceptions;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetById
{
    public class GetEmailByIdQuerryHandler : IRequestHandler<GetEmailByIdQuerry, EmailNotificationDto>
    {
        private readonly IEmailsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public GetEmailByIdQuerryHandler(
            IEmailsRepository repository,
            IMapper mapper,
            IUserContext userContext
            )
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<EmailNotificationDto> Handle(GetEmailByIdQuerry request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();
            var email = await _repository.GetEmailByIdAndUserIdAsync(request.Id, currentUser.Id);

            if (email == null)
            {
                throw new NotFoundException($"Email with id {request.Id} not found");
            }

            var dto = _mapper.Map<EmailNotificationDto>(email);
            return dto;
        }
    }
}