using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace NotificationService.Extensions.Swagger;

public static class ServiceCollectionExtension
{
    public static void AddSwaggerServiceCollection(this IServiceCollection services)
    {
       services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService", Version = "v1" });
       
           var securityScheme = new OpenApiSecurityScheme
           {
               Name = "JWT Authentication",
               Description = "Enter JWT Bearer token **_only_**",
               In = ParameterLocation.Header,
               Type = SecuritySchemeType.Http,
               Scheme = "bearer",
               BearerFormat = "JWT",
               Reference = new OpenApiReference
               {
                   Id = JwtBearerDefaults.AuthenticationScheme,
                   Type = ReferenceType.SecurityScheme
               },
           };
       
           c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
           {
               Description = "ApiKey must appear in header",
               Type = SecuritySchemeType.ApiKey,
               Name = "X-Api-Key",
               In = ParameterLocation.Header,
               Scheme = "ApiKeyScheme"
           });
       
           var apiKeySecurityScheme = new OpenApiSecurityScheme()
           {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "ApiKey"
               },
               In = ParameterLocation.Header
           };
           c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
       
           c.AddSecurityRequirement(new OpenApiSecurityRequirement
           {
               { securityScheme, new string[] { } }
           });
           c.AddSecurityRequirement(new OpenApiSecurityRequirement
           {
               { apiKeySecurityScheme, new string[] { } }
           });
       });

    }
}