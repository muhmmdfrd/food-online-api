using FoodOnline.Core.Helpers;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Services;
using FoodOnline.Repository.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodOnline.Core;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserSessionService, UserSessionService>();
    }

    public static void RegisterHelpers(this IServiceCollection services)
    {
        services.AddScoped<AuthHelper>();
        services.AddScoped<UserHelper>();
        services.AddScoped<UserSessionHelper>();
    }
    
    public static void AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }
}