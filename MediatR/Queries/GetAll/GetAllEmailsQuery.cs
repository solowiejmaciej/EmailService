using MediatR;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Models.Dtos;
using NotificationService.Models.Pagination;

namespace NotificationService.MediatR.Queries.GetAll
{
    public record GetAllEmailsQuery : IRequest<PageResult<EmailNotificationDto>>
    {
        public string? SearchPhrase { get; set; }
        public EQueryNotificationStatus? Status { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}