namespace NotificationService.Models.QueryParameters.GetAll;

public class GetAllUsersQueryParameters
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}