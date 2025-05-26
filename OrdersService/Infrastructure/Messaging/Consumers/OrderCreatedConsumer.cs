using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Products.UpdateStockQuantity;

namespace OrdersService.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer(IMediator mediator) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        var command = new UpdateStockQuantityCommand(message.CartItems);
        await mediator.Send(command, context.CancellationToken);
    }
}