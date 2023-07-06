using System.Reflection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Health;
using NotificationService.MappingProfiles;
using NotificationService.MappingProfiles.Notifications;
using NotificationService.MappingProfiles.Recipient;
using NotificationService.Middleware;
using NotificationService.Models.AppSettings;
using NotificationService.Models.Requests;
using NotificationService.Models.Validation.RequestValidation;
using NotificationService.Repositories;
using NotificationService.Services;
using NotificationService.Services.Notifications;
using NotificationService.UserContext;

namespace NotificationService.Extensions.Notifications
{
    public static class ServiceCollectionExtension
    {
        public static void AddNotificationsServiceCollection(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var redisOptions = configuration.GetSection("RedisSettings");
            var smtpConfig = configuration.GetSection(nameof(SMTPConfig));
            var googleFirebaseSettings = configuration.GetSection("GoogleFirebase");
            var smsSettings = configuration.GetSection("SmsSettings");

            // Add services to the container.

            //Helpers
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

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
            services.AddScoped<IRecipientService, RecipientService>();

            //Config
            services.Configure<SMTPConfig>(smtpConfig);
            services.Configure<RedisConfig>(redisOptions);
            services.Configure<GoogleFirebaseConfig>(googleFirebaseSettings);
            services.Configure<SmsConfig>(smsSettings);

            //My services
            services.AddScoped<IEmailDataService, EmailService>();

            //Middleware
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<ApiKeyAuthMiddleware>();

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
                    cfg.AddProfile(new RecipientMappingProfile());
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