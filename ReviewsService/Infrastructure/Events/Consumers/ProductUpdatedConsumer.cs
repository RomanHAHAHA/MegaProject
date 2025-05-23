using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Products.Update;

namespace ReviewsService.Infrastructure.Events.Consumers;

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