using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NotificationService.Extensions;
using NotificationService.Health;
using NotificationService.Middleware;
using NotificationService.Repositories;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEmailService();

var notificationAppSettings = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var hangfireConfig = notificationAppSettings.GetSection("HangfireSettings");

builder.Services.AddLogging();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService", Version = "v1" });

    // Konfiguracja autoryzacji JWT
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
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "EmailServiceHangfireDashboard",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter()
        {
            User = hangfireConfig["UserName"],
            Pass = hangfireConfig["Password"]
        }
    }
});
app.MapHangfireDashboard();

RecurringJob.AddOrUpdate<IEmailsRepository>("Delete emails", x => x.DeleteEmails(), Cron.Never);

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();