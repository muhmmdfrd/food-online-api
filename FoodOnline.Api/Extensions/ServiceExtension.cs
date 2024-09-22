using System.IO.Compression;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Asp.Versioning;
using FoodOnline.Api.Commons;
using FoodOnline.Api.Constants;
using FoodOnline.Api.Models;
using FoodOnline.Core.Helpers;
using FoodOnline.Core.Services.RedisCaching;
using FoodOnline.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FoodOnline.Api.Extensions;

public static class ServiceExtension
{
    public static void ConfigureApiControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ValidateModelStateAttribute));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var result = new ObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Code = ResponseConstant.BAD_REQUEST_CODE,
                    Message = ResponseConstant.BAD_REQUEST_MESSAGE,
                    Data = context.ModelState.Values.SelectMany(c => c.Errors).Select(x => x.ErrorMessage)
                });

                result.ContentTypes.Add(MediaTypeNames.Application.Json);

                return result;
            };
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        });
    }

    public static void ConfigureResponseCompression(this IServiceCollection services)
    {
        services.AddResponseCompression();
        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsConstant.CORS_NAME, builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader();
            });
        });
    }

    public static void ConfigureControllerPrefix(this IServiceCollection services)
    {
        var routeAttribute = new RouteAttribute("api");
        var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
        services.AddControllersWithViews(options => options.Conventions.Add(routePrefixConvention));
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    public static void ConfigureJwtAuthentication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var jwtConfigs = serviceProvider.GetRequiredService<IOptions<JwtConfigs>>().Value;
        var key = Encoding.ASCII.GetBytes(jwtConfigs.TokenSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfigs.Issuer,
                ValidAudience = jwtConfigs.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = _ => Task.CompletedTask,
                OnChallenge = context =>
                {
                    context.Response.ContentType = AppConstant.ApplicationJson;
                    
                    if (string.IsNullOrEmpty(context.Request.Headers.Authorization) || !string.IsNullOrEmpty(context.Error))
                    {
                        var response = new ApiResponse<object>().Unauthorized().ToString();
                        return context.Response.WriteAsync(response);
                    }
                    
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    if (string.IsNullOrEmpty(context.Request.Headers.Authorization))
                    {
                        context.Fail("Unauthorized.");
                        return Task.CompletedTask;
                    }
                    
                    var jwt = context.SecurityToken as JsonWebToken;
                    var claim = jwt?.Claims.FirstOrDefault(c => c.Type == "sessionCode");
                    var code = claim?.Value;

                    if (string.IsNullOrWhiteSpace(code))
                    {
                        context.Fail("Code not found.");
                        return Task.CompletedTask;
                    }
                    
                    var helper = context.HttpContext.RequestServices.GetRequiredService<UserSessionHelper>();
                    var isValid = helper.CheckCode(code);
                    if (isValid)
                    {
                        return Task.CompletedTask;
                    }
                    
                    context.Fail("Forbidden.");
                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
    }
    
    public static void AddJsonEnv(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder
                .Configuration
                .AddJsonFile("appsettings.json", true, true);
            return;
        }

        builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token in the text input below"
            });

            var openApiRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };

            c.AddSecurityRequirement(openApiRequirement);
        });
    }
    
    
    public static void RegisterAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfigs>(configuration.GetSection(nameof(JwtConfigs)));
        services.Configure<RedisConfigs>(configuration.GetSection(nameof(RedisConfigs)));
    }

    public static void RegisterRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("RedisConfigs:ConnectionString").Value;
        services.AddSingleton(new RedisService(connectionString!));
    }
}