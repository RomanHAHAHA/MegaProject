using Common.API.Extensions;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using Common.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrdersService.Application.Features.Orders.Create;
using OrdersService.Application.Options;
using OrdersService.Application.Services;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;
using OrdersService.Infrastructure.Messaging.Consumers;
using OrdersService.Infrastructure.Persistence;
using OrdersService.Infrastructure.Persistence.Repositories.Base;
using OrdersService.Infrastructure.Persistence.Repositories.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssembly(
    typeof(DeliveryLocationCreateDto).Assembly,
    includeInternalTypes: true);

builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<NovaPoshtaOptions>(builder.Configuration);

builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.Decorate<IOrdersRepository, LoggingOrdersRepository>();

builder.Services.AddScoped<IProductRepository, ProductsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IOutboxMessagesRepository, OutboxMessagesRepository<OrdersDbContext>>();

builder.Services.AddHttpClient<ICartServiceClient, CartServiceClient>();
builder.Services.AddHttpClient<INovaPoshtaClient, NovaPoshtaClient>();

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!;
var customCookieOptions = builder.Configuration.GetSection(nameof(CustomCookieOptions)).Get<CustomCookieOptions>()!;

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();
builder.Services.AddApiAuthorization(jwtOptions, customCookieOptions);

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.AddConsumer<ProductCreatedConsumer>();
    bugConfigurator.AddConsumer<ProductUpdatedConsumer>();
    bugConfigurator.AddConsumer<ProductMainImageSetConsumer>();
    bugConfigurator.AddConsumer<UserAvatarUpdatedConsumer>();
    bugConfigurator.AddConsumer<UserRegisteredConsumer>();
    
    bugConfigurator.SetKebabCaseEndpointNameFormatter();
    bugConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:UserName"]!);
            h.Password(builder.Configuration["MessageBroker:Password"]!);
        });
        
        configurator.ConfigureEndpoints(context);
        configurator.ReceiveEndpoint("orders-product-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("orders-product-updated", e =>
        {
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("orders-product-main-image-set", e =>
        {
            e.ConfigureConsumer<ProductMainImageSetConsumer>(context);
        });
        configurator.ReceiveEndpoint("orders-user-avatar-updated", e =>
        {
            e.ConfigureConsumer<UserAvatarUpdatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("orders-user-registered", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(context);
        });
    });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);
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