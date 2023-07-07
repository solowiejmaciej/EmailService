using MediatR;
using NotificationService.Models.Responses;

namespace NotificationService.MediatR.Queries.GetToken;

public record GetTokenQuerry : IRequest<TokenResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}