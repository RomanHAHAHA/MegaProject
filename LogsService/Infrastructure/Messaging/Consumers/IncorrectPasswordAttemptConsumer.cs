using Common.Infrastructure.Messaging.Events;
using LogsService.Application.Features.ActionLogs.LogIncorrectPassword;
using MassTransit;
using MediatR;

namespace LogsService.Infrastructure.Messaging.Consumers;

public class IncorrectPasswordAttemptConsumer(
    IMediator mediator) : IConsumer<IncorrectPasswordAttemptEvent>
{
    public async Task Consume(ConsumeContext<IncorrectPasswordAttemptEvent> context)
    {
        var @event = context.Message;
        var command = new LogIncorrectPasswordAttemptCommand(@event.UserId);
        await mediator.Send(command, context.CancellationToken);
    }
}