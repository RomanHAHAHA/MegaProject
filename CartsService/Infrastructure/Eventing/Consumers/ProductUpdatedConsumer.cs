using CartsService.Application.Features.Products.Update;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class ProductUpdatedConsumer(IMediator mediator) : IConsumer<ProductUpdatedEvent>
{
    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        var @event = context.Message;
        var command = new UpdateProductCommand(
            @event.Id,
            @event.Name,
            @event.Price);
        
        await mediator.Send(command, context.CancellationToken);
    }
}