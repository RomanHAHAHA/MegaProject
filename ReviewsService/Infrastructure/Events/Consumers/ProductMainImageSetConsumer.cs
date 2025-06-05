using Common.Infrastructure.Messaging.Events;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Products.SetMainImage;

namespace ReviewsService.Infrastructure.Events.Consumers;

public class ProductMainImageSetConsumer(IMediator mediator) : IConsumer<ProductMainImageSetEvent> 
{
    public async Task Consume(ConsumeContext<ProductMainImageSetEvent> context)
    {
        var @event = context.Message;
        var command = new SetMainProductImageCommand(
            @event.CorrelationId,
            @event.ImagePath);
        
        await mediator.Send(command, context.CancellationToken);
    }
}