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
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetUserByIdQuerryHandler(
            IUserRepository repository,
            IMapper mapper
            )
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuerry request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);
            if (user is null)
            {
                throw new NotFoundException($"User with id {request.Id} not found");
            }
            var dto = _mapper.Map<UserDto>(user);
            return dto;
        }
    }
}