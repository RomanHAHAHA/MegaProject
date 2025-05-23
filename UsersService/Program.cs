using Common.API.Extensions;
using Common.Application.Options;
using Common.Application.Services;
using Common.Domain.Interfaces;
using Common.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UsersService.Application.Features.Users.GetPagedList;
using UsersService.Application.Features.Users.Login;
using UsersService.Application.Features.Users.Register;
using UsersService.Application.Features.Users.SetAvatarImage;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;
using UsersService.Infrastructure.Messaging.Consumers;
using UsersService.Infrastructure.Persistence;
using UsersService.Infrastructure.Persistence.Repositories.Base;
using UsersService.Infrastructure.Persistence.Repositories.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<UserImagesOptions>(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(
    typeof(UserRegisterDto).Assembly,
    includeInternalTypes: true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHttpUserContext, HttpUserContext>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.Decorate<IUsersRepository, LoggingUsersRepository>();

builder.Services.Decorate<IFilterStrategy<User, UsersFilter>, UsersFilterStrategy>();
builder.Services.Decorate<ISortStrategy<User>, UsersSortStrategy>();

builder.Services.AddScoped<IOutboxMessagesRepository, OutboxMessagesRepository<UserDbContext>>();

builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddTransient<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<IFileStorageService, FileStorageService>();
builder.Services.AddTransient<UserFactory>();

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.AddConsumer<EmailConfirmedConsumer>();
    
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
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly);
});

var jwtOptions = builder.Configuration
    .GetSection(nameof(JwtOptions))
    .Get<JwtOptions>()!;

var customCookieOptions = builder.Configuration
    .GetSection(nameof(CustomCookieOptions))
    .Get<CustomCookieOptions>()!;

//builder.Services.AddSingleton<GlobalExceptionHandlingMiddleware>();
builder.Services.AddApiAuthorization(jwtOptions, customCookieOptions);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();