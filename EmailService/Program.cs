using Common.Application.Services;
using Common.Domain.Interfaces;
using Common.Infrastructure.Persistence.Repositories;
using EmailService.Application.Handlers;
using EmailService.Application.Options;
using EmailService.Application.Services;
using EmailService.Domain.Interfaces;
using EmailService.Infrastructure.Eventing.Consumers;
using EmailService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(nameof(SmtpOptions)));

builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddTransient<IVerificationCodeGenerator, VerificationCodeGenerator>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<ICacheService<string>, CacheService<string>>();
builder.Services.AddScoped<IOutboxMessagesRepository, OutboxMessagesRepository<EmailConfirmationDbContext>>();

builder.Services.AddDbContext<EmailConfirmationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

//builder.Services.AddSingleton<GlobalExceptionHandlingMiddleware>();

builder.Services.AddMassTransit(bugConfigurator =>
{
    bugConfigurator.AddConsumer<UserRegisteredEventConsumer>();
    
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
    cfg.RegisterServicesFromAssembly(typeof(ConfirmEmailCommandHandler).Assembly);
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