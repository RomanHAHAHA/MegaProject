using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Products.Reserve;

namespace ProductsService.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer(IMediator mediator) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var command = new ReserveOrderProductsCommand(
            context.Message.OrderId,
            context.Message.UserId,
            context.Message.CartItems);
        
        await mediator.Send(command, context.CancellationToken);
    }
}