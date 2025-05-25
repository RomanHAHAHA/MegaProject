using Common.API.Extensions;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ReviewsService.Application.Features.Reviews.Create;
using ReviewsService.Application.Services;
using ReviewsService.Domain.Interfaces;
using ReviewsService.Infrastructure.Events.Consumers;
using ReviewsService.Infrastructure.Persistence;
using ReviewsService.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);

builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ReviewsFactory>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();
builder.Services.AddHttpClient<IOrderServiceClient, OrderServiceClient>();

builder.Services.AddDbContext<ReviewsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddApiAuthorization(builder.Configuration);

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.AddConsumers(typeof(Program).Assembly);
    bugConfigurator.SetKebabCaseEndpointNameFormatter();
    
    bugConfigurator.AddEntityFrameworkOutbox<ReviewsDbContext>(options =>
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
        configurator.ReceiveEndpoint("reviews-product-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("reviews-product-updated", e =>
        {
            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("reviews-product-main-image-set", e =>
        {
            e.ConfigureConsumer<ProductMainImageSetConsumer>(context);
        });
        configurator.ReceiveEndpoint("reviews-user-avatar-updated", e =>
        {
            e.ConfigureConsumer<UserAvatarUpdatedConsumer>(context);
        });
        configurator.ReceiveEndpoint("reviews-user-registered", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(context);
        });
        configurator.ReceiveEndpoint("reviews-product-deleted", e =>
        {
            e.ConfigureConsumer<ProductDeletedConsumer>(context);
        });
    });
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateReviewCommandHandler).Assembly);
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