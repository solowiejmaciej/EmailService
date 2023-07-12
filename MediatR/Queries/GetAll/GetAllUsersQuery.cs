using MediatR;
using NotificationService.Models.Dtos;
using NotificationService.Models.Pagination;

namespace NotificationService.MediatR.Queries.GetAll
{
    public record GetAllUsersQuery : IRequest<PageResult<UserDto>>
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}