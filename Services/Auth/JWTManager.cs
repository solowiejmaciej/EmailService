using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Entities;
using NotificationService.Exceptions;
using NotificationService.Models.AppSettings;
using NotificationService.Models.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using crypto;
using Microsoft.EntityFrameworkCore;
using NotificationService.MediatR.Commands.RefreshToken;
using NotificationService.MediatR.Queries.GetToken;

namespace NotificationService.Services.Auth;

public interface IJWTManager
{
    TokenResponse GenerateJWT(ApplicationUser applicationUser);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand command);
}

public class JwtManager : IJWTManager
{
    private readonly NotificationDbContext _dbContext;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IOptions<JWTSettings> _jwtAppSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly TokenValidationParameters _refreshTokenValidationParameters;

    public JwtManager(
        NotificationDbContext dbcontext,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IOptions<JWTSettings> config, 
        TokenValidationParameters tokenValidationParameters,
        TokenValidationParameters refreshTokenValidationParameters
        
        )
    {
        _jwtAppSettings = config;
        _tokenValidationParameters = tokenValidationParameters;
        _refreshTokenValidationParameters = refreshTokenValidationParameters;
        _dbContext = dbcontext;
        _passwordHasher = passwordHasher;
    }

    public TokenResponse GenerateJWT(ApplicationUser dbUser)
    {
        
        var expires = DateTime.Now.AddMinutes(_jwtAppSettings.Value.ExpireMinutes);

        var userRoleId = _dbContext.UserRoles.FirstOrDefault(r => r.UserId == dbUser.Id)!.RoleId;
        var userRoleName = _dbContext.Roles.FirstOrDefault(r => r.Id == userRoleId)!.Name;

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, dbUser.Id),
                new Claim(ClaimTypes.Name, dbUser.Email!),
                new Claim(ClaimTypes.Role, $"{userRoleName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        string rsaPrivateKey = File.ReadAllText(@"certi\privateKey.pem");
        using var rsa = RSA.Create();
        rsa.ImportFromPem(rsaPrivateKey);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var jwt = new JwtSecurityToken(
            audience: _jwtAppSettings.Value.Issuer,
            issuer: _jwtAppSettings.Value.Issuer,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
            );

        var refreshToken = new RefreshToken()
        {
            JwtId = jwt.Id,
            UserId = dbUser.Id,
            CreationDate = DateTime.Now,
            ExpiryDate = DateTime.Now.AddMonths(6),
            Token = Guid.NewGuid().ToString(),
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();
        
        var response = new TokenResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            StatusCode = 200,
            IssuedDate = DateTime.Now,
            ExpiresAt = expires,
            Role = userRoleName,
            RoleId = userRoleId,
            UserId = dbUser.Id,
            RefreshToken = refreshToken.Token
        };

        return response;
    }

    private ClaimsPrincipal getPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, _refreshTokenValidationParameters, out var validatedToken);
            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
                StringComparison.InvariantCultureIgnoreCase
               );
    }
    
    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand command)
    {
        var validatedToken = getPrincipalFromToken(command.Token);

        if (validatedToken is null)
        {
            throw new BadRequestException("Invalid Token");
        }

        var exipryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        var expiryDateTime = new DateTime(1970, 1, 1, 0, 0, 0)
            .AddSeconds(exipryDateUnix);

        if (expiryDateTime > DateTime.Now)
        {
            throw new BadRequestException("Token hasn't expired yet");
        }

        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        var storedRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == command.RefreshToken);
        if (storedRefreshToken is null)
        {
            throw new BadRequestException("Token doesn't exists");
        }

        if (DateTime.Now > storedRefreshToken.ExpiryDate)
        {
            throw new BadRequestException("Token has expired");
        }

        if (storedRefreshToken.Invalidated)
        {
            throw new BadRequestException("Token has been invalidated");
        }

        if (storedRefreshToken.Used)
        {
            throw new BadRequestException("Token has been used");
        }
        
        if (storedRefreshToken.JwtId != jti)
        {
            throw new BadRequestException("This refresh token does not match this JWT");
        }

        storedRefreshToken.Used = true;
        _dbContext.RefreshTokens.Update(storedRefreshToken);
        await _dbContext.SaveChangesAsync();

        var userId = validatedToken.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        return GenerateJWT(user);
    }
}