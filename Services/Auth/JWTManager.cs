using NotificationService.Models.AppSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using NotificationService.Entities;
using NotificationService.Exceptions;
using NotificationService.Models.Requests;
using NotificationService.Models.Responses;

namespace NotificationService.Services.Auth;

public interface IJWTManager
{
    TokenResponse GenerateJWT(UserLoginRequest user);
}

public class JwtManager : IJWTManager
{
    private readonly NotificationDbContext _dbContext;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IOptions<JWTConfig> _jwtAppSettings;

    public JwtManager(NotificationDbContext dbcontext, IPasswordHasher<ApplicationUser> passwordHasher, IOptions<JWTConfig> config)
    {
        _jwtAppSettings = config;
        _dbContext = dbcontext;
        _passwordHasher = passwordHasher;
    }

    public TokenResponse GenerateJWT(UserLoginRequest dto)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Email == dto.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var expires = DateTime.Now.AddMinutes(_jwtAppSettings.Value.JwtExpireMinutes);

        var userRoleId = _dbContext.UserRoles.FirstOrDefault(r => r.UserId == user.Id)!.RoleId;
        var userRoleName = _dbContext.Roles.FirstOrDefault(r => r.Id == userRoleId)!.Name;

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, $"{userRoleName}")
            };

        string rsaPrivateKey = File.ReadAllText(@"certi\privateKey.pem");
        using var rsa = RSA.Create();
        rsa.ImportFromPem(rsaPrivateKey);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var jwt = new JwtSecurityToken(
            audience: _jwtAppSettings.Value.JwtIssuer,
            issuer: _jwtAppSettings.Value.JwtIssuer,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var response = new TokenResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            StatusCode = 200,
            IssuedDate = DateTime.Now,
            ExpiresAt = expires,
            Role = userRoleName,
            RoleId = userRoleId,
            UserId = user.Id
        };

        return response;
    }
}