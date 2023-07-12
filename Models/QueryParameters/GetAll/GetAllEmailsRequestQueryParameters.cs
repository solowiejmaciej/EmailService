using NotificationService.Entities.NotificationEntities;
using NotificationService.Models.Pagination;

namespace NotificationService.Models.QueryParameters.GetAll;

public class GetAllEmailsRequestQueryParameters
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public EQueryNotificationStatus? Status { get; set; } = null;

}