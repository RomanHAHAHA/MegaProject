using CartsService.Application.Features.Products.Delete;
using Common.Infrastructure.Messaging.Events;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class ProductSnapshotCreationFailedConsumer(
    IMediator mediator) : IConsumer<ProductSnapshotCreationFailedEvent>
{
    public async Task Consume(ConsumeContext<ProductSnapshotCreationFailedEvent> context)
    {
        var @event = context.Message;
        var command = new DeleteProductCommand(@event.ProductId);
        await mediator.Send(command, context.CancellationToken);
    }
}