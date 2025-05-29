using Common.Domain.Enums;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using OrdersService.API.Hubs;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.Confirm;

public class ConfirmOrderProcessingCommandHandler(
    IOrdersRepository ordersRepository,
    IPublishEndpoint publisher,
    IHubContext<OrdersHub, IOrdersClient> hubContext) : IRequestHandler<ConfirmOrderProcessingCommand>
{
    public async Task Handle(ConfirmOrderProcessingCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            await hubContext.Clients
                .User(request.UserId.ToString())
                .OrderFailed("Unexpected error occured during processing of the order");
                
            return;
        }

        order.Status = OrderStatus.Reserved;
        var saved = await ordersRepository.SaveChangesAsync(cancellationToken);

        if (!saved)
        {
            await hubContext.Clients
                .User(request.UserId.ToString())
                .OrderFailed("Unexpected error occured during processing of the order"); 
            
            return;
        }

        await OnOrderProcessed(order.UserId, cancellationToken);
        
        await hubContext.Clients
            .User(request.UserId.ToString())
            .OrderProcessed(order.UserId, "Order processed successfully");
    }

    private async Task OnOrderProcessed(Guid userId, CancellationToken cancellationToken)
    {
        await publisher.Publish(new OrderProcessedEvent(userId), cancellationToken);
    }
}