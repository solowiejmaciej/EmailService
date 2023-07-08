using AutoMapper;
using MediatR;
using NotificationService.Exceptions;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetById
{
    public class GetSmsByIdQuerryHandler : IRequestHandler<GetSmsByIdQuerry, SmsNotificationDto>
    {
        private readonly ISmsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public GetSmsByIdQuerryHandler(
            ISmsRepository repository,
            IMapper mapper,
            IUserContext userContext
            )
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<SmsNotificationDto> Handle(GetSmsByIdQuerry request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();
            var sms = await _repository.GetSmsByIdAndUserIdAsync(request.Id, currentUser.Id, cancellationToken);
            var dto = _mapper.Map<SmsNotificationDto>(sms);

            if (sms == null)
            {
                throw new NotFoundException($"Sms with id {request.Id} not found");
            }

            return dto;
        }
    }
}