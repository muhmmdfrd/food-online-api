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
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderDetailService, OrderDetailService>();
        services.AddScoped<IOrderPaymentService, OrderPaymentService>();
    }

    public static void RegisterHelpers(this IServiceCollection services)
    {
        services.AddScoped<AuthHelper>();
        services.AddScoped<UserHelper>();
        services.AddScoped<UserSessionHelper>();
        services.AddScoped<RoleHelper>();
        services.AddScoped<PositionHelper>();
        services.AddScoped<OrderHelper>();
        services.AddScoped<OrderDetailHelper>();
        services.AddScoped<OrderPaymentHelper>();
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