using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Products.Update;

namespace OrdersService.Infrastructure.Messaging.Consumers;

public class ProductUpdatedConsumer(IMediator mediator) : IConsumer<ProductUpdatedEvent>
{
    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        var @event = context.Message;
        var command = new UpdateProductCommand(
            @event.Id,
            @event.Name,
            @event.Price,
            @event.StockQuantity);
        
        await mediator.Send(command, context.CancellationToken);
    }
}