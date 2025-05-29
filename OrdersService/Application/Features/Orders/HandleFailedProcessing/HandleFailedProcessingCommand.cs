using Common.Infrastructure.Messaging.Events;
using MediatR;

namespace OrdersService.Application.Features.Orders.HandleFailedProcessing;

public record HandleFailedProcessingCommand(
    Guid OrderId,
    Guid UserId,
    List<ProductStockInfo> OutOfStockProductIds) : IRequest;