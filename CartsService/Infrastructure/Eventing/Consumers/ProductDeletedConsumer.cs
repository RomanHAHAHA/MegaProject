using CartsService.Application.Features.Products.Delete;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class ProductDeletedConsumer(IMediator mediator) : IConsumer<ProductDeletedEvent>
{
    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        var command = new DeleteProductCommand(context.Message.ProductId);
        await mediator.Send(command, context.CancellationToken);
    }
}