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