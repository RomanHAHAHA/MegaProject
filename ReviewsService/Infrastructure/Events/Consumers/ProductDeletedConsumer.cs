using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Products.Delete;

namespace ReviewsService.Infrastructure.Events.Consumers;

public class ProductDeletedConsumer(IMediator mediator) : IConsumer<ProductDeletedEvent>
{
    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        var command = new DeleteProductCommand(context.Message.ProductId);
        await mediator.Send(command, context.CancellationToken);
    }
}