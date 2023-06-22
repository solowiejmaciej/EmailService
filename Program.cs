using EmailService.Middleware;
using EmailService.Services;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using EmailService.Repositories;
using AuthService.Extensions;

using EmailService.Extensions;

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

builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailService", Version = "v1" });

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

RecurringJob.AddOrUpdate<IEmailSenderService>("Send background emails job", x => x.SendInBackground(), Cron.Never);
RecurringJob.AddOrUpdate<IEmailSenderService>("Add Test email to DB", x => x.AddTestEmail(), Cron.Never);
RecurringJob.AddOrUpdate<IEmailsRepository>("Delete emails", x => x.DeleteEmails(), Cron.Never);

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();