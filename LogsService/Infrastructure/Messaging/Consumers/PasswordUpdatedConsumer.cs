using Common.Infrastructure.Messaging.Events;
using LogsService.Application.Features.ActionLogs.LogPasswordUpdated;
using MassTransit;
using MediatR;

namespace LogsService.Infrastructure.Messaging.Consumers;

public class PasswordUpdatedConsumer(IMediator mediator) : IConsumer<PasswordUpdatedEvent>
{
    public async Task Consume(ConsumeContext<PasswordUpdatedEvent> context)
    {
        var @event = context.Message;
        var command = new LogPasswordUpdatedCommand(@event.UserId);
        await mediator.Send(command, context.CancellationToken);
    }
}