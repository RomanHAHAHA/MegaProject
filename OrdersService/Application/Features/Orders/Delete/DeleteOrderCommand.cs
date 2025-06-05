using Common.Infrastructure.Messaging.Events.Product;
using MediatR;

namespace OrdersService.Application.Features.Orders.Delete;

public record DeleteOrderCommand(
    Guid OrderId,
    List<ProductStockInfo> ProductStockInfos) : IRequest;