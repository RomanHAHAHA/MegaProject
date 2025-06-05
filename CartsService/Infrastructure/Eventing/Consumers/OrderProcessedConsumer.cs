using CartsService.Application.Features.CartItems.CleanCart;
using Common.Infrastructure.Messaging.Events.Order;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class OrderProcessedConsumer(IMediator mediator) : IConsumer<OrderProcessedEvent>
{
    public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
    {
        var @event = context.Message;
        var command = new CleanCartCommand(@event.UserId);
        await mediator.Send(command, context.CancellationToken);
    }
}