using MediatR;
using Microsoft.AspNetCore.SignalR;
using OrdersService.API.Hubs;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.HandleFailedProcessing;

public class HandleFailedProcessingCommandHandler(
    IOrdersRepository ordersRepository,
    IHubContext<OrdersHub, IOrdersClient> hubContext) : IRequestHandler<HandleFailedProcessingCommand>
{
    public async Task Handle(HandleFailedProcessingCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            await hubContext.Clients
                .User(request.UserId.ToString())
                .OrderFailed("Unexpected error occured during processing of the order");
            
            return;
        }

        ordersRepository.Delete(order);
        var saved = await ordersRepository.SaveChangesAsync(cancellationToken);

        if (saved)
        {
            await hubContext.Clients
                .User(request.UserId.ToString())
                .NotEnoughProductsOnStock(request.OutOfStockProductIds);
        }
        else
        {
            await hubContext.Clients
                .User(request.UserId.ToString())
                .OrderFailed("Unexpected error occured during processing of the order");
        }
    }
}