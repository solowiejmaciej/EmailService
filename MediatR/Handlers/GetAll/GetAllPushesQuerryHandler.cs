using AutoMapper;
using MediatR;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllPushesQuerryHandler : IRequestHandler<GetAllPushesQuerry, List<PushNotificationDto>>
    {
        private readonly IPushRepository _repository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public GetAllPushesQuerryHandler(
            IPushRepository repository,
            IUserContext userContext,
            IMapper mapper
            )
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<List<PushNotificationDto>> Handle(GetAllPushesQuerry request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            var emails = await _repository.GetAllPushesToUserIdAsync(currentUser.Id, cancellationToken);
            var dtos = _mapper.Map<List<PushNotificationDto>>(emails);
            return dtos;
        }
    }
}