using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Products.SetMainImage;

namespace OrdersService.Infrastructure.Messaging.Consumers;

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