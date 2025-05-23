using CartsService.Application.Features.Products.Create;
using CartsService.Domain.Interfaces;
using CartsService.Infrastructure.Eventing.Consumers;
using CartsService.Infrastructure.Persistence;
using CartsService.Infrastructure.Persistence.Repositories.Base;
using Common.API.Extensions;
using Common.Application.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);

builder.Services.AddScoped<IProductRepository, ProductsRepository>();
builder.Services.AddScoped<ICartsRepository, CartsRepository>();

builder.Services.AddDbContext<CartsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
});

var jwtOptions = builder.Configuration
    .GetSection(nameof(JwtOptions))
    .Get<JwtOptions>() ?? throw new NullReferenceException(nameof(JwtOptions));

var customCookieOptions = builder.Configuration
    .GetSection(nameof(CustomCookieOptions))
    .Get<CustomCookieOptions>() ?? throw new NullReferenceException(nameof(CustomCookieOptions));

builder.Services.AddApiAuthorization(jwtOptions, customCookieOptions);

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.AddConsumer<ProductCreatedConsumer>();
    bugConfigurator.AddConsumer<ProductUpdatedConsumer>();
    bugConfigurator.AddConsumer<ProductMainImageSetConsumer>();
    bugConfigurator.AddConsumer<OrderCreatedConsumer>();
    
    bugConfigurator.SetKebabCaseEndpointNameFormatter();
    bugConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:UserName"]!);
            h.Password(builder.Configuration["MessageBroker:Password"]!);
        });
        
        configurator.ConfigureEndpoints(context);
        configurator.ReceiveEndpoint("carts-product-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("carts-product-updated", e =>
        {
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("carts-product-main-image-set", e =>
        {
            e.ConfigureConsumer<ProductMainImageSetConsumer>(context);
        });
    });
});

builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();