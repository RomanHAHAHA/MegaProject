using CartsService.Application.Features.CartItems.CleanCart;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace CartsService.Infrastructure.Eventing.Consumers;

public class OrderCreatedConsumer(IMediator mediator) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var @event = context.Message;
        var command = new CleanCartCommand(@event.UserId);
        await mediator.Send(command, context.CancellationToken);
    }
}