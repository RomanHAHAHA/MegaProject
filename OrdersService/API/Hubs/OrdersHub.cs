using Common.Infrastructure.Messaging.Events;
using Microsoft.AspNetCore.SignalR;

namespace OrdersService.API.Hubs;

public interface IOrdersClient
{
    Task OrderProcessed(Guid orderId, string description);
    
    Task OrderFailed(string reason);

    Task NotEnoughProductsOnStock(List<ProductStockInfo> products);
}

public class OrdersHub : Hub<IOrdersClient>
{
}