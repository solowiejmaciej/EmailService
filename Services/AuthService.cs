using EmailService.Entities;
using EmailService.Exceptions;
using EmailService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmailService.Services;

public interface IAuthService
{
    string GenerateJWT(UserDto user);

    void AddNewUser(UserDto user);
}

public class AuthService : IAuthService
{
    private EmailsDbContext _dbcontext { get; }
    private IPasswordHasher<User> _passwordHasher { get; }
    private JwtAppSettings _jwtAppSettings { get; }

    public AuthService(EmailsDbContext dbcontext, IPasswordHasher<User> passwordHasher, JwtAppSettings jwtSettings)
    {
        _jwtAppSettings = jwtSettings;
        _dbcontext = dbcontext;
        _passwordHasher = passwordHasher;
    }


    public void AddNewUser(UserDto user)
    {
        var newUser = new User
        {
            Login = user.Login
        };

        var hashedPass = _passwordHasher.HashPassword(newUser, user.Password);
        newUser.PasswordHashed = hashedPass;

        _dbcontext.Users.Add(newUser);
        _dbcontext.SaveChanges();
    }

    public string GenerateJWT(UserDto dto)
    {
        var user = _dbcontext.Users
                    .FirstOrDefault(x => x.Login == dto.Login);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAppSettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_jwtAppSettings.JwtExpireDays);
        var token = new JwtSecurityToken(_jwtAppSettings.JwtIssuer,
            _jwtAppSettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: cred);
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);

    }
}

