using Common.API.Extensions;
using Common.API.Middlewares;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Features.ProductImages.Create;
using ProductsService.Application.Features.Products.Create;
using ProductsService.Application.Features.Products.GetPagedList;
using ProductsService.Application.Services;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;
using ProductsService.Infrastructure.Messaging.Consumers;
using ProductsService.Infrastructure.Persistence;
using ProductsService.Infrastructure.Persistence.Repositories;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ProductsService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
        builder.Services.AddScoped<IFilterStrategy<Product, ProductFilter>, ProductFilterStrategy>();
        builder.Services.AddScoped<ISortStrategy<Product>, ProductSortStrategy>();

        builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        builder.Services.AddScoped<IProductImagesRepository, ProductImagesRepository>();
        builder.Services.AddScoped<IProductCharacteristicsRepository, ProductCharacteristicsRepository>();

        builder.Services.AddDbContext<ProductsDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
        });

        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
        
        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();
        builder.Services.AddApiAuthorization(builder.Configuration);

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddFluentValidationAutoValidation();

        builder.Services.AddTransient<IFileStorageService, FileStorageService>();
        
        builder.Services.AddSingleton<GlobalExceptionHandlingMiddleware>();
        builder.Services.AddHttpClient<IReviewsClient, ReviewsClient>();
        
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
        });

        return builder;
    }

    public static WebApplicationBuilder AddOptionsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
        builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);
        builder.Services.AddConfiguredOptions<ProductImagesOptions>(builder.Configuration);

        return builder;
    }

    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(bugConfigurator =>
        {
            bugConfigurator.AddConsumers(typeof(Program).Assembly);
            
            bugConfigurator.AddEntityFrameworkOutbox<ProductsDbContext>(options =>
            {
                options.QueryDelay = TimeSpan.FromSeconds(1);
                options.UseSqlServer().UseBusOutbox();
            });
    
            bugConfigurator.SetKebabCaseEndpointNameFormatter();
            bugConfigurator.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(builder.Configuration["MessageBroker:UserName"]!);
                    h.Password(builder.Configuration["MessageBroker:Password"]!);
                });
        
                configurator.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}