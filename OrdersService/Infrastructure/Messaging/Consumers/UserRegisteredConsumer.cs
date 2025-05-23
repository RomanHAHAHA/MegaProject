using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Users.Create;

namespace OrdersService.Infrastructure.Messaging.Consumers;

public class UserRegisteredConsumer(IMediator mediator) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var @event = context.Message;
        var command = new CreateUserCommand(@event.UserId, @event.NickName);
        await mediator.Send(command, context.CancellationToken);
    }
}