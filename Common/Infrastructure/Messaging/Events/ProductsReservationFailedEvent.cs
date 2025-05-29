using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductsReservationFailedEvent(
    Guid OrderId, 
    Guid UserId,
    List<ProductStockInfo> ProductStockInfos) : BaseEvent;

public record ProductStockInfo(Guid ProductId, int StockQuantity);
