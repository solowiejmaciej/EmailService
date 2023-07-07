using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NotificationService.Extensions.Auth;
using NotificationService.Extensions.Events;
using NotificationService.Extensions.Hangfire;
using NotificationService.Extensions.Notifications;
using NotificationService.Extensions.Users;
using NotificationService.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthServiceCollection();
builder.Services.AddNotificationsServiceCollection();
builder.Services.AddHangfireServiceCollection();
builder.Services.AddAzureServiceBus();
builder.Services.AddUsersServiceCollection();

builder.Services.AddLogging();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "apiCorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                //                .AllowCredentials()
                .SetIsOriginAllowed(options => true);
            //.WithMethods("OPTIONS", "GET");
        });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseHangfire();

app.MapHangfireDashboard();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();
//app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseStatusCodePages();
app.Run();