using AutoMapper;
using MediatR;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Models.Pagination;
using NotificationService.Repositories;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PageResult<UserDto>>
    {
        private readonly IUsersRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(
            IUsersRepository userRepository,
            IMapper mapper
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        // TODO: Create GenericType PaginationService 

        public async Task<PageResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            var baseQuery = users
                .Where(e => request.SearchPhrase == null ||
                            (e.Firstname!.ToLower().Contains(request.SearchPhrase.ToLower())
                             || e.Surname!.ToLower().Contains(request.SearchPhrase.ToLower())
                             || e.PhoneNumber!.ToLower().Contains(request.SearchPhrase.ToLower())
                             || e.Email!.ToLower().Contains(request.SearchPhrase.ToLower())
                            ));

            var queryResult = baseQuery
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<UserDto>>(queryResult);

            var totalItemsCount = queryResult.Count;

            var result = new PageResult<UserDto>(dtos, totalItemsCount, request.PageSize, request.PageNumber);
            return result;
        }
    }
}