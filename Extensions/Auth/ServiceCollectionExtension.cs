using NotificationService.Models.Validation.RequestValidation;

namespace NotificationService.Extensions.Auth;

using Models.AppSettings;
using Models.Requests;
using Services;
using UserContext;
using Models.Validation;
using NotificationService.Services.Auth;
using Services.Users;
using Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

public static class ServiceCollectionExtension
{
    public static void AddAuthServiceCollection(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        JWTConfig jwtConfig = new JWTConfig();
        var jwtAppSettings = configuration.GetSection("Auth");
        jwtAppSettings.Bind(jwtConfig);

        services.Configure<JWTConfig>(jwtAppSettings);

        // Add services to the container.

        //My services
        services.AddScoped<IJWTManager, JwtManager>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IUserContext, UserContext>();

        //Validation
        services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidation>();
        services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterRequestValidation>();

        //PasswordHasher
        services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(
                source: Convert.FromBase64String(jwtConfig.JwtPublicKey),
                bytesRead: out int _
            );
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                IssuerSigningKey = new RsaSecurityKey(rsa),
            };
        });
    }
}