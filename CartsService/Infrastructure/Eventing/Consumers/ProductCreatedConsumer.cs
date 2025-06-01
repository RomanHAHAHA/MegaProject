using CartsService.Application.Features.Products.Create;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class ProductCreatedConsumer(IMediator mediator) : IConsumer<ProductCreatedEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var @event = context.Message;
        var command = new CreateProductCommand(
            @event.CorrelationId,
            @event.ProductId,
            @event.SellerId,
            @event.Name,
            @event.Price);

        await mediator.Send(command, context.CancellationToken);
    }
}