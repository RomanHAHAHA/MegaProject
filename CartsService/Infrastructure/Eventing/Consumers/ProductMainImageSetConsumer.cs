using CartsService.Application.Features.Products.SetMainImage;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class ProductMainImageSetConsumer(IMediator mediator) : IConsumer<ProductMainImageSetEvent> 
{
    public async Task Consume(ConsumeContext<ProductMainImageSetEvent> context)
    {
        var @event = context.Message;
        var command = new SetMainProductImageCommand(
            @event.Id,
            @event.ImagePath);
        
        await mediator.Send(command, context.CancellationToken);
    }
}