using EmailService;
using EmailService.Entities;
using EmailService.Middleware;
using EmailService.Models;
using EmailService.Models.Validation;
using FluentValidation.AspNetCore;
using FluentValidation;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using EmailService.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var appSettings = new JwtAppSettings();

configuration.GetSection("Auth").Bind(appSettings);

var hangfireConfig = configuration.GetSection("HangfireSettings");
// Add services to the container.

builder.Services.AddLogging();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddDbContext<EmailsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("App"));
});

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("Hangfire"))
);


var smtpConfig = builder.Configuration.GetSection(nameof(SMTPConfig));

//My services
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();

//Helpers
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<EmailDto>, EmailDtoValidation>();
builder.Services.AddHangfireServer();

//Config
builder.Services.Configure<SMTPConfig>(smtpConfig);
builder.Services.AddSingleton(appSettings);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

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

RecurringJob.AddOrUpdate<IEmailSenderService>("Send background emails job", x => x.SendInBackground(), Cron.Minutely);
RecurringJob.AddOrUpdate<IEmailSenderService>("Add Test email to DB", x => x.AddTestEmail(), Cron.Never);

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();