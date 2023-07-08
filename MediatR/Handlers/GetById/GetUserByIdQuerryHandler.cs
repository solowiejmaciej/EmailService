using AutoMapper;
using MediatR;
using NotificationService.Exceptions;
using NotificationService.MediatR.Queries.GetById;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;

namespace NotificationService.MediatR.Handlers.GetById
{
    public class GetUserByIdQuerryHandler : IRequestHandler<GetUserByIdQuerry, UserDto>
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;

        public GetUserByIdQuerryHandler(
            IUsersRepository repository,
            IMapper mapper
            )
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuerry request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"User with id {request.Id} not found");
            }
            var dto = _mapper.Map<UserDto>(user);
            return dto;
        }
    }
}