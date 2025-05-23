using Common.API.Extensions;
using Common.API.Middlewares;
using Common.Application.Options;
using Common.Domain.Interfaces;
using LogsService.Application.Features.ActionLogs.GetLogsPagedList;
using LogsService.Application.Features.ActionLogs.LogCrudAction;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using LogsService.Infrastructure.Messaging.Consumers;
using LogsService.Infrastructure.Persistence;
using LogsService.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfiguredOptions<JwtOptions>(builder.Configuration);
builder.Services.AddConfiguredOptions<CustomCookieOptions>(builder.Configuration);

builder.Services.AddScoped<ILogsRepository, LogsRepository>();
builder.Services.AddScoped<IFilterStrategy<ActionLog, ActionLogFilter>, ActionLogFilterStrategy>();
builder.Services.AddScoped<ISortStrategy<ActionLog>, ActionLogSortStrategy>();

builder.Services.AddDbContext<ActionLogsDbContext>(options =>
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
    bugConfigurator.AddConsumer<DbActionPerformedConsumer>();
    bugConfigurator.AddConsumer<PasswordUpdatedConsumer>();
    bugConfigurator.AddConsumer<IncorrectPasswordAttemptConsumer>();
    
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
    cfg.RegisterServicesFromAssembly(typeof(CreateLogActionCommandHandler).Assembly);
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