using Common.Application.Options;
using Common.Domain.Dtos;
using Common.Domain.Enums;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.Order;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;
using OrdersService.Infrastructure.Persistence;

namespace OrdersService.Application.Features.Orders.Create;

public class CreateOrderCommandHandler(
    IOrdersRepository ordersRepository,
    IUsersRepository usersRepository,
    OrdersDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<CreateOrderCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CartItems.Count == 0)
        {
            return BaseResponse.BadRequest("You must provide at least one item.");
        }
        
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return BaseResponse.NotFound(nameof(UserSnapshot));
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var order = new Order(request.UserId);
        
        await ordersRepository.CreateAsync(order, cancellationToken);

        order.AddDeliveryLocation(request.DeliveryLocationDto);
        order.AddOrderItems(request.CartItems);
        
        await OnOrderCreated(order, request.CartItems, cancellationToken);

        var saved = await ordersRepository.SaveChangesAsync(cancellationToken);

        if (!saved)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError("Failed to create order");
        }

        await transaction.CommitAsync(cancellationToken);
        return BaseResponse.Ok();
    }

    private async Task OnOrderCreated(
        Order order,
        List<CartItemDto> cartItems, 
        CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var serviceName = serviceOptions.Value.Name;
        
        await publishEndpoint.Publish(new SystemActionEvent
        {
            CorrelationId = correlationId,
            SenderServiceName = serviceName,
            UserId = order.UserId,
            ActionType = ActionType.Create,
            Message = $"Order {order.Id} created"
        }, cancellationToken);
        
        await publishEndpoint.Publish(
            new OrderCreatedEvent
            {
                CorrelationId = correlationId,
                SenderServiceName = serviceName,
                UserId = order.UserId,
                OrderId = order.Id,
                CartItems = cartItems,
            }, 
            cancellationToken);
    }
}