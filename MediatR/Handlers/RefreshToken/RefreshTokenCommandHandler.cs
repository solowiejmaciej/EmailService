using MediatR;
using NotificationService.MediatR.Commands.RefreshToken;
using NotificationService.Models.Responses;
using NotificationService.Services.Auth;

namespace NotificationService.MediatR.Handlers.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IJWTManager _jwtManager;

    public RefreshTokenCommandHandler(
        IJWTManager jwtManager
        )
    {
        _jwtManager = jwtManager;
    }
    
    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
       return await _jwtManager.RefreshTokenAsync(request);
    }
}