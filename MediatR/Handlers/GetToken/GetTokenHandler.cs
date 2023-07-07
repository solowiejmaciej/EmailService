using MediatR;
using NotificationService.MediatR.Queries.GetToken;
using NotificationService.Models.Responses;
using NotificationService.Services.Auth;

namespace NotificationService.MediatR.Handlers.GetToken
{
}

public class GetTokenHandler : IRequestHandler<GetTokenQuerry, TokenResponse>
{
    private readonly IJWTManager _jwtManager;

    public GetTokenHandler(
        IJWTManager jwtManager
        )
    {
        _jwtManager = jwtManager;
    }

    public async Task<TokenResponse> Handle(GetTokenQuerry request, CancellationToken cancellationToken)
    {
        return _jwtManager.GenerateJWT(request);
    }
}