using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using UsersService.Application.Features.Users.MarkEmailConfirmed;

namespace UsersService.Infrastructure.Messaging.Consumers;

public class EmailConfirmedConsumer(IMediator mediator) : IConsumer<EmailConfirmedEvent>
{
    public async Task Consume(ConsumeContext<EmailConfirmedEvent> context)
    {
        var @event = context.Message;
        var command = new MarkEmailAsConfirmedCommand(@event.Email);
        await mediator.Send(command, context.CancellationToken);
    }
}