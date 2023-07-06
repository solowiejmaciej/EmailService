using AutoMapper;
using MediatR;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllSmsQuerryHandler : IRequestHandler<GetAllSmsQuerry, List<SmsNotificationDto>>
    {
        private readonly ISmsRepository _repository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public GetAllSmsQuerryHandler(
            ISmsRepository repository,
            IUserContext userContext,
            IMapper mapper
        )
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<List<SmsNotificationDto>> Handle(GetAllSmsQuerry request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            var emails = await _repository.GetAllSmsToUserIdAsync(currentUser.Id);
            var dtos = _mapper.Map<List<SmsNotificationDto>>(emails);
            return dtos;
        }
    }
}