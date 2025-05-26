using Common.API.Extensions;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ReviewsService.Application.Features.Reviews.Create;
using ReviewsService.Application.Services;
using ReviewsService.Domain.Interfaces;
using ReviewsService.Infrastructure.Events.Consumers;
using ReviewsService.Infrastructure.Persistence;
using ReviewsService.Infrastructure.Persistence.Repositories;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ReviewsService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

        builder.Services.AddDbContext<ReviewsDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddFluentValidationAutoValidation();
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();
        builder.Services.AddHttpClient<IOrderServiceClient, OrderServiceClient>();

        builder.Services.AddApiAuthorization(builder.Configuration);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateReviewCommandHandler).Assembly);
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
        builder.Services.AddMassTransit(bc =>
        {
            bc.AddConsumers(typeof(Program).Assembly);
            bc.SetKebabCaseEndpointNameFormatter();

            bc.AddEntityFrameworkOutbox<ReviewsDbContext>(o =>
            {
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UseSqlServer().UseBusOutbox();
            });

            bc.UsingRabbitMq((context, c) =>
            {
                c.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(builder.Configuration["MessageBroker:UserName"]!);
                    h.Password(builder.Configuration["MessageBroker:Password"]!);
                });

                c.ConfigureEndpoints(context);

                c.ReceiveEndpoint("reviews-product-created", e => e.ConfigureConsumer<ProductCreatedConsumer>(context));
                c.ReceiveEndpoint("reviews-product-updated", e => e.ConfigureConsumer<ProductUpdatedConsumer>(context));
                c.ReceiveEndpoint("reviews-product-main-image-set", e => e.ConfigureConsumer<ProductMainImageSetConsumer>(context));
                c.ReceiveEndpoint("reviews-user-avatar-updated", e => e.ConfigureConsumer<UserAvatarUpdatedConsumer>(context));
                c.ReceiveEndpoint("reviews-user-registered", e => e.ConfigureConsumer<UserRegisteredConsumer>(context));
                c.ReceiveEndpoint("reviews-product-deleted", e => e.ConfigureConsumer<ProductDeletedConsumer>(context));
            });
        });

        return builder;
    }
}