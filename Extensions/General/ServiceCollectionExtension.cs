using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Health;

namespace NotificationService.Extensions.General;

public static class ServiceCollectionExtension
{
    public static void AddGeneralServiceCollection(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddCors(options =>
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
        
        //Db
        services.AddDbContext<NotificationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("App"));
        });
        
        //Cache
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            string connection = configuration.GetConnectionString("Redis");
            redisOptions.Configuration = connection;
        });
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        
        services.AddMemoryCache();

        //HealthChecks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("mssqlDb")
            .AddCheck<CacheDbHealthCheck>("cache")
            .AddCheck<SmsPlanetApiHealthCheck>("smsPlanetApi");
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }
}