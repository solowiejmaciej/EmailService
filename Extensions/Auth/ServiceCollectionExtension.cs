namespace NotificationService.Extensions.Auth;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models.AppSettings;
using NotificationService.Middleware;
using System.Security.Cryptography;

public static class ServiceCollectionExtension
{
    public static void AddAuthServiceCollection(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var jwtSettings = new JWTSettings();
        var authConfigurationSection = configuration.GetSection("AuthSettings");
        authConfigurationSection.Bind(jwtSettings);

        var apiKeySettings = new ApiKeySettings();
        var apiKeyConfigurationSection = configuration.GetSection("ApiKeySettings");
        apiKeyConfigurationSection.Bind(apiKeySettings);

        RSA rsa = RSA.Create();
        
        rsa.ImportSubjectPublicKeyInfo(
            source: Convert.FromBase64String(jwtSettings.PublicKey),
            bytesRead: out int _
        );
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
            
        };
        
        var refreshTokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new RsaSecurityKey(rsa),
        };

        services.AddSingleton(tokenValidationParameters);
        services.AddSingleton(refreshTokenValidationParameters);
        
        services.Configure<ApiKeySettings>(apiKeyConfigurationSection);
        services.Configure<JWTSettings>(authConfigurationSection);

        services.AddScoped<ApiKeyAuthMiddleware>();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = tokenValidationParameters;
        });
    }
}