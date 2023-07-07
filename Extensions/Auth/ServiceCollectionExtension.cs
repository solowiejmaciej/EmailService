namespace NotificationService.Extensions.Auth;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models.AppSettings;
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