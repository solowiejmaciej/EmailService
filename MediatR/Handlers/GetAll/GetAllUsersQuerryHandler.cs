using AutoMapper;
using MediatR;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllUsersQuerryHandler : IRequestHandler<GetAllUsersQuerry, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQuerryHandler(
            IUserRepository userRepository,
            IMapper mapper
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersQuerry request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = _mapper.Map<List<UserDto>>(users);
            return dtos;
        }
    }
}