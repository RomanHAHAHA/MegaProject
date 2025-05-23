using Common.API.Extensions;
using Common.API.Middlewares;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using Common.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Features.Categories.Common;
using ProductsService.Application.Features.ProductImages.Create;
using ProductsService.Application.Features.Products.Common;
using ProductsService.Application.Features.Products.Create;
using ProductsService.Application.Features.Products.GetPagedList;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;
using ProductsService.Infrastructure.Persistence;
using ProductsService.Infrastructure.Persistence.Repositories.Base;
using ProductsService.Infrastructure.Persistence.Repositories.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<ProductImagesOptions>(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();

builder.Services.AddValidatorsFromAssembly(
    typeof(ProductCreateDto).Assembly,
    includeInternalTypes: true);

builder.Services.AddTransient<ProductFactory>();
builder.Services.AddTransient<CategoryFactory>();
builder.Services.AddTransient<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.Decorate<IProductsRepository, LoggingProductsRepository>();
builder.Services.AddScoped<IFilterStrategy<Product, ProductFilter>, ProductFilterStrategy>();
builder.Services.AddScoped<ISortStrategy<Product>, ProductSortStrategy>();

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.Decorate<ICategoriesRepository, LoggingCategoriesRepository>();

builder.Services.AddScoped<IProductImagesRepository, ProductImagesRepository>();
builder.Services.Decorate<IProductImagesRepository, LoggingProductImagesRepository>();

builder.Services.AddScoped<IOutboxMessagesRepository, OutboxMessagesRepository<ProductsDbContext>>();

builder.Services.AddDbContext<ProductsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

var jwtOptions = builder.Configuration
    .GetSection(nameof(JwtOptions))
    .Get<JwtOptions>() ?? throw new NullReferenceException(nameof(JwtOptions));

var customCookieOptions = builder.Configuration
    .GetSection(nameof(CustomCookieOptions))
    .Get<CustomCookieOptions>() ?? throw new NullReferenceException(nameof(CustomCookieOptions));

builder.Services.AddApiAuthorization(jwtOptions, customCookieOptions);
builder.Services.AddSingleton<GlobalExceptionHandlingMiddleware>();

builder.Services.AddMassTransit(bugConfigurator =>
{
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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();