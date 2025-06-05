using Common.Infrastructure.Messaging.Events;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Products.Delete;

namespace ProductsService.Infrastructure.Messaging.Consumers;

public class ProductSnapshotCreationFailedConsumer(
    IMediator mediator) : IConsumer<ProductSnapshotCreationFailedEvent>
{
    public async Task Consume(ConsumeContext<ProductSnapshotCreationFailedEvent> context)
    {
        Console.WriteLine("Delete product");
        var @event = context.Message;
        var command = new DeleteProductCommand(@event.ProductId);
        await mediator.Send(command, context.CancellationToken);
    }
}