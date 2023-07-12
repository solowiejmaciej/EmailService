using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Entities;
using NotificationService.Entities.NotificationEntities;
using NotificationService.MediatR.Queries.GetAll;
using NotificationService.Models.Dtos;
using NotificationService.Models.Pagination;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.GetAll
{
    public class GetAllEmailsQueryHandler : IRequestHandler<GetAllEmailsQuery, PageResult<EmailNotificationDto>>
    {
        private readonly IUserContext _userContext;
        private readonly IEmailsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllEmailsQueryHandler(
            IUserContext userContext,
            IEmailsRepository repository,
            IMapper mapper
            )
        {
            _userContext = userContext;
            _repository = repository;
            _mapper = mapper;
        }

        // TODO: Create GenericType PaginationService 
        public async Task<PageResult<EmailNotificationDto>> Handle(GetAllEmailsQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();

            var emails = await _repository.GetAllEmailsToUserIdAsync(currentUser.Id, cancellationToken);
            var baseQuery = emails
                .Where(e => request.SearchPhrase == null ||
                            (e.Subject.ToLower().Contains(request.SearchPhrase.ToLower())
                             || e.Content.ToLower().Contains(request.SearchPhrase.ToLower())
                            ));

            if (!(request.Status.ToString().IsNullOrEmpty()))
            {
                baseQuery = baseQuery.Where(
                    e => e.Status == Enum.Parse<EStatus>(request.Status.ToString()!)
                );
            }
            
            
            var emailsResult = baseQuery
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .ToList();

            var emailsDtos = _mapper.Map<List<EmailNotificationDto>>(emailsResult);

            var totalItemsCount = emailsResult.Count;

            var result = new PageResult<EmailNotificationDto>(emailsDtos, totalItemsCount, request.PageSize, request.PageNumber);

            return result;
        }
    }
}