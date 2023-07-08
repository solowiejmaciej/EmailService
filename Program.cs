using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NotificationService.Extensions.Auth;
using NotificationService.Extensions.Events;
using NotificationService.Extensions.General;
using NotificationService.Extensions.Hangfire;
using NotificationService.Extensions.Notifications;
using NotificationService.Extensions.Swagger;
using NotificationService.Extensions.Users;
using NotificationService.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Custom ServiceCollections
builder.Services.AddAuthServiceCollection();
builder.Services.AddNotificationsServiceCollection();
builder.Services.AddHangfireServiceCollection();
builder.Services.AddAzureServiceBus();
builder.Services.AddUsersServiceCollection();
builder.Services.AddGeneralServiceCollection();
builder.Services.AddSwaggerServiceCollection();


builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);



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
app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseStatusCodePages();
app.Run();