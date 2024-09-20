using Flozacode.Repository;
using FoodOnline.Api.Extensions;
using FoodOnline.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Host.UseSerilog((context, services, configuration) => {
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.AddJsonEnv();

var services = builder.Services;
services.AddFlozaRepo();
services.AddSwagger();
services.AddVersioning();
services.ConfigureResponseCompression();
services.AddResponseCaching();
services.ConfigureApiControllers();
services.ConfigureControllerPrefix();
services.AddDbContext(builder.Configuration);
services.RegisterServices();
services.RegisterHelpers();
services.RegisterAppSettings(builder.Configuration);
services.RegisterRedis(builder.Configuration);
services.ConfigureCors();
services.ConfigureJwtAuthentication();
services.AddEndpointsApiExplorer();
services.ConfigureAutoMapper();
services.AddHttpContextAccessor();

// App builder
var app = builder.Build();
app.RegisterCors();
app.RegisterSwagger();
app.RegisterMiddlewares();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();