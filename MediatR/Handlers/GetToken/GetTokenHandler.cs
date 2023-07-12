using MediatR;
using Microsoft.AspNetCore.Identity;
using NotificationService.Entities;
using NotificationService.Exceptions;
using NotificationService.MediatR.Queries.GetToken;
using NotificationService.Models.Responses;
using NotificationService.Repositories;
using NotificationService.Services.Auth;

namespace NotificationService.MediatR.Handlers.GetToken
{
}

public class GetTokenHandler : IRequestHandler<GetTokenQuerry, TokenResponse>
{
    private readonly IJWTManager _jwtManager;
    private readonly NotificationDbContext _dbContext;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public GetTokenHandler(
        IJWTManager jwtManager, NotificationDbContext dbContext, IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _jwtManager = jwtManager;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<TokenResponse> Handle(GetTokenQuerry request, CancellationToken cancellationToken)
    {
        
        var user = _dbContext.Users.FirstOrDefault(x => x.Email == request.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }
        
        return _jwtManager.GenerateJWT(user);
    }
}