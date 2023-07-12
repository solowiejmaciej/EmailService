using AutoMapper;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Entities.NotificationEntities;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Models.Pagination;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllPushesQueryHandler : IRequestHandler<GetAllPushesQuery, PageResult<PushNotificationDto>>
    {
        private readonly IPushRepository _repository;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public GetAllPushesQueryHandler(
            IPushRepository repository,
            IUserContext userContext,
            IMapper mapper
            )
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }
        // TODO: Create GenericType PaginationService 
        public async Task<PageResult<PushNotificationDto>> Handle(GetAllPushesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            var pushes = await _repository.GetAllPushesToUserIdAsync(currentUser.Id, cancellationToken);
            var baseQuery = pushes
                .Where(e => request.SearchPhrase == null ||
                            (e.Title.ToLower().Contains(request.SearchPhrase.ToLower())
                             || e.Content.ToLower().Contains(request.SearchPhrase.ToLower())
                            ));

            if (!(request.Status.ToString().IsNullOrEmpty()))
            {
                baseQuery = baseQuery.Where(
                    e => e.Status == Enum.Parse<EStatus>(request.Status.ToString()!)
                );
            }
            
            
            var queryResult = baseQuery
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<PushNotificationDto>>(queryResult);

            var totalItemsCount = queryResult.Count;

            var result = new PageResult<PushNotificationDto>(dtos, totalItemsCount, request.PageSize, request.PageNumber);
            return result;
        }
    }
}