using AutoMapper;
using MediatR;
using NotificationService.MediatR.Queries;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllEmailsQuerryHandler : IRequestHandler<GetAllEmailsQuerry, List<EmailNotificationDto>>
    {
        private readonly IUserContext _userContext;
        private readonly IEmailsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllEmailsQuerryHandler(
            IUserContext userContext,
            IEmailsRepository repository,
            IMapper mapper
            )
        {
            _userContext = userContext;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<EmailNotificationDto>> Handle(GetAllEmailsQuerry request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            var emails = await _repository.GetAllEmailsToUserIdAsync(currentUser.Id);
            var dtos = _mapper.Map<List<EmailNotificationDto>>(emails);
            return dtos;
        }
    }
}