using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Orders.HandleFailedProcessing;

namespace OrdersService.Infrastructure.Messaging.Consumers;

public class ProductsReservationFailedConsumer(IMediator mediator) : IConsumer<ProductsReservationFailedEvent>
{
    public async Task Consume(ConsumeContext<ProductsReservationFailedEvent> context)
    {
        var command = new HandleFailedProcessingCommand(
            context.Message.OrderId,
            context.Message.UserId,
            context.Message.ProductStockInfos);
        
        await mediator.Send(command, context.CancellationToken);
    }
}