using AuthService.Extensions;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using AuthService.UserContext;
using NotificationService.Entities;
using NotificationService.MappingProfiles;
using NotificationService.Middleware;
using NotificationService.Models;
using NotificationService.Models.AppSettings;
using NotificationService.Models.Validation;
using NotificationService.Repositories;
using NotificationService.Services;
using NotificationService.Hangfire;
using NotificationService.Health;
using NotificationService.Models.Dtos;
using NotificationService.Models.Requests;

namespace NotificationService.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddEmailService(this IServiceCollection services)
        {
            services.AddCustomHangfire();
            services.AddAuthService();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var redisOptions = configuration.GetSection("RedisSettings");
            var smtpConfig = configuration.GetSection(nameof(SMTPConfig));
            var googleFirebaseSettings = configuration.GetSection("GoogleFirebase");
            var smsSettings = configuration.GetSection("SmsSettings");

            // Add services to the container.

            //Db
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("App"));
            });

            //Cache
            services.AddScoped<ICacheService, CacheService>();

            //Repos
            services.AddScoped<IEmailsRepository, EmailsRepository>();
            services.AddScoped<IPushRepository, PushRepository>();
            services.AddScoped<ISmsRepository, SmsRepository>();

            //Config
            services.Configure<SMTPConfig>(smtpConfig);
            services.Configure<RedisConfig>(redisOptions);
            services.Configure<GoogleFirebaseConfig>(googleFirebaseSettings);
            services.Configure<SmsConfig>(smsSettings);

            //My services
            services.AddScoped<IEmailDataService, EmailService>();
            services.AddScoped<IPushDataService, PushService>();
            services.AddScoped<ISmsService, SmsService>();

            //Middleware
            services.AddScoped<ErrorHandlingMiddleware>();

            //Validation
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<EmailRequest>, EmailRequestValidation>();
            services.AddScoped<IValidator<SmsRequest>, SmsRequestValidation>();
            services.AddScoped<IValidator<PushRequest>, PushRequestValidation>();

            //Mapper
            services.AddScoped(provider => new MapperConfiguration(cfg =>
                {
                    var scope = provider.CreateScope();
                    var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();
                    cfg.AddProfile(new EmailMappingProfile(userContext));
                    cfg.AddProfile(new PushMappingProfile(userContext));
                    cfg.AddProfile(new SmsMappingProfile(userContext));
                }).CreateMapper()
            );

            //HealthChecks
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("mssqlDb")
                .AddCheck<CacheDbHealthCheck>("cache")
                .AddCheck<SmsPlanetApiHealthCheck>("smsPlanetApi");
        }
    }
}