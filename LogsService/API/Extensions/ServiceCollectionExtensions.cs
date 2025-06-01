using Common.API.Extensions;
using Common.API.Filters;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using LogsService.Application.Features.ActionLogs.GetLogsPagedList;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using LogsService.Infrastructure.Persistence;
using LogsService.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LogsService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ILogsRepository, LogsRepository>();
        builder.Services.AddScoped<IFilterStrategy<ActionLog, ActionLogFilter>, ActionLogFilterStrategy>();
        builder.Services.AddScoped<ISortStrategy<ActionLog>, ActionLogSortStrategy>();

        builder.Services.AddDbContext<ActionLogsDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
        });

        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
        
        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
        });
        builder.Services.AddScoped<ICacheService<object>, CacheService<object>>();
        
        builder.Services.AddApiAuthorization(builder.Configuration);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        return builder;
    }

    public static WebApplicationBuilder AddOptionsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
        builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);

        return builder;
    }

    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(bugConfigurator =>
        {
            bugConfigurator.AddConsumers(typeof(Program).Assembly);
            bugConfigurator.SetKebabCaseEndpointNameFormatter();
    
            bugConfigurator.AddEntityFrameworkOutbox<ActionLogsDbContext>(options =>
            {
                options.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                options.UseSqlServer();
            });
    
            bugConfigurator.UsingRabbitMq((context, с) =>
            {
                с.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(builder.Configuration["MessageBroker:UserName"]!);
                    h.Password(builder.Configuration["MessageBroker:Password"]!);
                });
                
                с.UseConsumeFilter(typeof(IdempotencyFilter<>), context);
                с.ConfigureEndpoints(context);
            });
        });
        
        return builder;
    }
}