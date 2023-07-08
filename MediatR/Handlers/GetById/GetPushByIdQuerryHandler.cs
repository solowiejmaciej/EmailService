using AutoMapper;
using MediatR;
using NotificationService.Exceptions;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetById
{
    public class GetPushByIdQuerryHandler : IRequestHandler<GetPushByIdQuerry, PushNotificationDto>
    {
        private readonly IPushRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public GetPushByIdQuerryHandler(
            IPushRepository repository,
            IMapper mapper,
            IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<PushNotificationDto> Handle(GetPushByIdQuerry request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();
            var push = await _repository.GetPushByIdAndUserIdAsync(request.Id, currentUser.Id, cancellationToken);

            if (push == null)
            {
                throw new NotFoundException($"Push with id {request.Id} not found");
            }

            var dto = _mapper.Map<PushNotificationDto>(push);
            return dto;
        }
    }
}