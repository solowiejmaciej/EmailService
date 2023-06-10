using AuthService.Extensions;
using AuthService.Models;
using AutoMapper;
using EmailService.ApplicationUser;
using EmailService.Entities;
using EmailService.MappingProfiles;
using EmailService.Middleware;
using EmailService.Models.AppSettings;
using EmailService.Models.Validation;
using EmailService.Models;
using EmailService.Repositories;
using EmailService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddEmailService(this IServiceCollection services)
        {
            services.AddAuthService();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var redisOptions = configuration.GetSection("RedisSettings");
            var smtpConfig = configuration.GetSection(nameof(SMTPConfig));
            var hangfireConfig = configuration.GetSection("HangfireSettings");
            var googleFirebaseSettings = configuration.GetSection("GoogleFirebase");

            // Add services to the container.

            //Db
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("App"));
            });

            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IEmailsRepository, EmailsRepository>();
            services.AddScoped<IPushRepository, PushRepository>();

            //Hangfire
            services.AddHangfire(config => config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("Hangfire"))
            );
            //Config
            services.Configure<SMTPConfig>(smtpConfig);
            services.Configure<RedisConfig>(redisOptions);
            services.Configure<GoogleFirebaseConfig>(googleFirebaseSettings);

            //My services
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IPushSenderService, PushSenderService>();
            services.AddScoped<IEmailDataService, EmailDataService>();
            services.AddScoped<IPushDataService, PushDataService>();

            //Middleware
            services.AddScoped<ErrorHandlingMiddleware>();

            //Helpers
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<EmailDto>, EmailDtoValidation>();
            services.AddHangfireServer();
            services.AddScoped(provider => new MapperConfiguration(cfg =>
                {
                    var scope = provider.CreateScope();
                    var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();
                    cfg.AddProfile(new EmailMappingProfile(userContext));
                    cfg.AddProfile(new PushMappingProfile(userContext));
                }).CreateMapper()
            );
        }
    }
}