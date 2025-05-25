using CartsService.Application.Features.Products.Create;
using CartsService.Domain.Interfaces;
using CartsService.Infrastructure.Eventing.Consumers;
using CartsService.Infrastructure.Persistence;
using CartsService.Infrastructure.Persistence.Repositories;
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

builder.Services.AddApiAuthorization(builder.Configuration);

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.SetKebabCaseEndpointNameFormatter();
    bugConfigurator.AddConsumers(typeof(Program).Assembly);
    
    bugConfigurator.AddEntityFrameworkOutbox<CartsDbContext>(options =>
    {
        options.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        options.UseSqlServer();
    });
    
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
        configurator.ReceiveEndpoint("carts-product-deleted", e =>
        {
            e.ConfigureConsumer<ProductDeletedConsumer>(context);
        });
    });
});

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