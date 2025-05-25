using Common.API.Extensions;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
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
using OrdersService.Infrastructure.Persistence.Repositories;

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

builder.Services.AddScoped<IProductRepository, ProductsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddHttpClient<ICartServiceClient, CartServiceClient>();
builder.Services.AddHttpClient<INovaPoshtaClient, NovaPoshtaClient>();

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();
builder.Services.AddApiAuthorization(builder.Configuration);

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.AddConsumers(typeof(Program).Assembly);
    bugConfigurator.SetKebabCaseEndpointNameFormatter();
    
    bugConfigurator.AddEntityFrameworkOutbox<OrdersDbContext>(options =>
    {
        options.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        options.QueryDelay = TimeSpan.FromSeconds(1);
        options.UseSqlServer().UseBusOutbox();
    });
    
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
        configurator.ReceiveEndpoint("orders-product-deleted", e =>
        {
            e.ConfigureConsumer<ProductDeletedConsumer>(context);
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