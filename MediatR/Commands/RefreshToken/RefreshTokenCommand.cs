using MediatR;
using NotificationService.Models.Responses;

namespace NotificationService.MediatR.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<TokenResponse>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}