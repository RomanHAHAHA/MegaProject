using Common.API.Filters;
using Common.Application.Services;
using Common.Domain.Interfaces;
using EmailService.Application.Features.EmailConfirmations.ConfirmEmail;
using EmailService.Application.Features.EmailConfirmations.SendCode;
using EmailService.Domain.Interfaces;
using MassTransit;

namespace EmailService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
        builder.Services.AddTransient<IVerificationCodeGenerator, VerificationCodeGenerator>();
        builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
        });
        builder.Services.AddScoped<ICacheService<string>, CacheService<string>>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ConfirmEmailCommandHandler).Assembly);
        });

        return builder;
    }

    public static WebApplicationBuilder AddOptionsServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(nameof(SmtpOptions)));

        return builder;
    }

    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(bugConfigurator =>
        {
            bugConfigurator.AddConsumers(typeof(Program).Assembly);
            bugConfigurator.SetKebabCaseEndpointNameFormatter();
    
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