using System.Reflection;
using System.Text;
using Common.API.Authentication;
using Common.Application.Options;
using Common.Domain.Interfaces;
using Common.Infrastructure.Messaging.Idempotency;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Common.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfiguredOptions<T>(
        this IServiceCollection services, 
        IConfiguration configuration) where T : class
    {
        services.Configure<T>(configuration.GetSection(typeof(T).Name));
    }
    
    public static void AddApiAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {   
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;
        var cookieOptions = configuration.GetSection(nameof(CustomCookieOptions)).Get<CustomCookieOptions>()!;
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[cookieOptions.Name];
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
    }
    
    public static void RegisterIdempotentConsumers(this IServiceCollection services, Assembly assembly)
    {
        var consumerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(i => new { ConsumerInterface = i, Implementation = t }))
            .ToList();

        foreach (var c in consumerTypes)
        {
            services.AddScoped(c.Implementation); 

            var decoratorType = typeof(IdempotentEventConsumer<>)
                .MakeGenericType(c.ConsumerInterface.GetGenericArguments()[0]);

            services.AddScoped(c.ConsumerInterface, provider =>
            {
                var innerConsumer = provider.GetRequiredService(c.Implementation);
                var cacheService = provider.GetRequiredService<ICacheService<object>>();
                var loggerType = typeof(ILogger<>).MakeGenericType(decoratorType);
                var logger = provider.GetRequiredService(loggerType);

                return Activator.CreateInstance(decoratorType, innerConsumer, cacheService, logger)!;
            });
        }
    }
}