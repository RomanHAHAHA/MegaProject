using Common.Infrastructure.Messaging.Events.CartItem;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Products.CheckStockQuantity;

namespace ProductsService.Infrastructure.Messaging.Consumers;

public class ProductCartQuantityIncrementedConsumer(
    IMediator mediator) : IConsumer<ProductCartQuantityIncrementedEvent>
{
    public async Task Consume(ConsumeContext<ProductCartQuantityIncrementedEvent> context)
    {
        var @event = context.Message;
        var command = new CheckProductStockQuantityCommand(
            @event.UserId,
            @event.ProductId, 
            @event.RequestedQuantity);
        await mediator.Send(command, context.CancellationToken);
    }
}