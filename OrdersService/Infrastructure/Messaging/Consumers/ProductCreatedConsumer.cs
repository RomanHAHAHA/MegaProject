using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Products.Create;

namespace OrdersService.Infrastructure.Messaging.Consumers;

public class ProductCreatedConsumer(IMediator mediator) : IConsumer<ProductCreatedEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var @event = context.Message;
        var command = new CreateProductCommand(
            @event.Id,
            @event.Name,
            @event.Price);
        
        await mediator.Send(command, context.CancellationToken);
    }
}