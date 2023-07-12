using MediatR;
using NotificationService.Models.Dtos;
using NotificationService.Models.Pagination;

namespace NotificationService.MediatR.Queries.GetAll
{
    public record GetAllSmsQuery : IRequest<PageResult<SmsNotificationDto>>
    {
        public string? SearchPhrase { get; set; }
        public EQueryNotificationStatus? Status { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}