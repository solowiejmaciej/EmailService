using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Health;
using NotificationService.MappingProfiles.Notifications;
using NotificationService.MappingProfiles.Recipients;
using NotificationService.Middleware;
using NotificationService.Models.AppSettings;
using NotificationService.Models.QueryParameters;
using NotificationService.Models.Requests;
using NotificationService.Models.Validation.QueryParametersValidation;
using NotificationService.Models.Validation.RequestValidation;
using NotificationService.Repositories;
using NotificationService.Services;
using NotificationService.Services.Notifications;
using System.Reflection;
using NotificationService.MappingProfiles.User;
using NotificationService.Models.Requests.Update;

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

            //Middleware
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<ApiKeyAuthMiddleware>();

            //Validation
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<AddEmailRequest>, AddEmailRequestValidation>();
            services.AddScoped<IValidator<AddSmsRequest>, AddSmsRequestValidation>();
            services.AddScoped<IValidator<AddPushRequest>, AddPushRequestValidation>();

            services.AddScoped<IValidator<EmailRequestQuerryParameters>, EmailRequestQuerryParametersValidation>();
            services.AddScoped<IValidator<SmsRequestQuerryParameters>, SmsRequestQuerryParametersValidation>();
            services.AddScoped<IValidator<PushRequestQuerryParameters>, PushRequestQuerryParametersValidation>();

            services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidation>();

            //Mapper
            services.AddScoped(provider => new MapperConfiguration(cfg =>
                {
                    //var scope = provider.CreateScope();
                    //var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();
                    cfg.AddProfile(new EmailMappingProfile());
                    cfg.AddProfile(new PushMappingProfile());
                    cfg.AddProfile(new SmsMappingProfile());
                    cfg.AddProfile(new RecipientMappingProfile());
                    cfg.AddProfile(new UserMappingProfile());
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