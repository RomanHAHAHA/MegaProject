using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;
using OrdersService.Infrastructure.Persistence;

namespace OrdersService.Application.Features.Orders.Create;

public class CreateOrderCommandHandler(
    IOrdersRepository ordersRepository,
    IUsersRepository usersRepository,
    OrdersDbContext dbContext,
    IValidator<DeliveryLocationCreateDto> validator,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateOrderCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        CreateOrderCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request.DeliveryLocationDto);

        if (!validationResult.IsValid)
        {
            return BaseResponse.BadRequest(validationResult);
        }

        if (request.CartItems.Count == 0)
        {
            return BaseResponse.BadRequest("You must provide at least one item.");
        }
        
        var user = await usersRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return BaseResponse.NotFound(nameof(UserSnapshot));
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var order = new Order(request.UserId);
        var created = await ordersRepository.CreateAsync(order, cancellationToken);

        if (!created)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError("Failed to create order");
        }

        order.AddDeliveryLocation(request.DeliveryLocationDto);
        order.AddOrderItems(request.CartItems);
        
        var updated = await ordersRepository.UpdateAsync(order, cancellationToken);

        if (!updated)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError("Failed to update order");
        }
        
        await transaction.CommitAsync(cancellationToken);
        await OnOrderCreated(request.UserId, cancellationToken);
        
        return BaseResponse.Ok();
    }

    private async Task OnOrderCreated(Guid userId, CancellationToken cancellationToken)
    {
        var orderCreatedEvent = new OrderCreatedEvent(userId);
        await publishEndpoint.Publish(orderCreatedEvent, cancellationToken);
    }
}