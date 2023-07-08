using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NotificationService.Entities;
using NotificationService.Models.Requests;
using NotificationService.Models.Requests.Update;
using NotificationService.Models.Validation.RequestValidation;
using NotificationService.Repositories;
using NotificationService.Repositories.Cached;
using NotificationService.Services.Auth;
using NotificationService.UserContext;

namespace NotificationService.Extensions.Users;

public static class ServiceCollectionExtension
{
    public static void AddUsersServiceCollection(this IServiceCollection services)
    {
        //My services
        services.AddScoped<IJWTManager, JwtManager>();
        services.AddScoped<IUserContext, UserContext.UserContext>();

        //Validation
        services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidation>();
        services.AddScoped<IValidator<AddUserRequest>, AddUserRequestValidation>();

        //PasswordHasher
        services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
        
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidation>();

        
        services.AddScoped<IUsersRepository,UsersRepository>();
        services.Decorate<IUsersRepository, CachedUsersRepository>();
    }
}