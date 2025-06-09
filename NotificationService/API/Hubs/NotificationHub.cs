using Common.Infrastructure.Messaging.Events.Product;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.API.Hubs;

public class NotificationHub : Hub<INotificationClient>
{
    public string GetConnectionId() => Context.ConnectionId;
}

public interface INotificationClient
{
    Task NotifyProductCreated(Guid productId, string message);
    
    Task NotifyProductCreationFailed(string error);
    
    Task NotifyUserRegistered();
    
    Task NotifyUserRegistrationFailed();
    
    Task NotifyUserAvatarUpdated(string message);

    Task NotifyUserAvatarUpdateFailed(string message);
    
    Task NotifyOrderProcessed(string message);
    
    Task NotifyProductsReservationFailed(List<ProductStockInfo> productStockInfos);
    
    Task NotifyProductStockExceeded(int stockQuantity);
}