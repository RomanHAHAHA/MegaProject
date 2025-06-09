using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Products.Delete;

namespace ProductsService.Infrastructure.Messaging.Consumers;

public class ProductSnapshotCreationFailedConsumer(
    IServiceProvider serviceProvider) : IConsumer<ProductSnapshotCreationFailedEvent>
{
    public async Task Consume(ConsumeContext<ProductSnapshotCreationFailedEvent> context)
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var @event = context.Message;
        var command = new DeleteProductCommand(@event.ProductId);
        await mediator.Send(command, context.CancellationToken);
    }
}