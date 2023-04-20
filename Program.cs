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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var appSettings = new JwtAppSettings();
var smtpConfig = builder.Configuration.GetSection(nameof(SMTPConfig));
var hangfireConfig = configuration.GetSection("HangfireSettings");

configuration.GetSection("Auth").Bind(appSettings);
// Add services to the container.

builder.Services.AddDbContext<EmailsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("App"));
});
builder.Services.AddScoped<Seeder>();

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("Hangfire"))
);

//My services
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();

//Helpers
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<EmailDto>, EmailDtoValidation>();
builder.Services.AddHangfireServer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddLogging();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

//Config
builder.Services.Configure<SMTPConfig>(smtpConfig);
builder.Services.AddSingleton(appSettings);

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

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = appSettings.JwtIssuer,
        ValidAudience = appSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtKey)),
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.

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

RecurringJob.AddOrUpdate<IEmailSenderService>("Send background emails job", x => x.SendInBackground(), Cron.Minutely);
RecurringJob.AddOrUpdate<IEmailSenderService>("Add Test email to DB", x => x.AddTestEmail(), Cron.Never);

app.MapHealthChecks("/health");

app.MapControllers();

SeedDatabase();

app.Run();

void SeedDatabase() //can be placed at the very bottom under app.Run()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<Seeder>();
        dbInitializer.Seed();
    }
}