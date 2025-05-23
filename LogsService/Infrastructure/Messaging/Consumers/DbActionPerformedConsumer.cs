using Common.Infrastructure.Messaging.Events;
using LogsService.Application.Features.ActionLogs.LogCrudAction;
using MassTransit;
using MediatR;

namespace LogsService.Infrastructure.Messaging.Consumers;

public class DbActionPerformedConsumer(IMediator mediator) : IConsumer<DbActionPerformedEvent>
{
    public async Task Consume(ConsumeContext<DbActionPerformedEvent> context)
    {
        var @event = context.Message;
        var command = new CreateLogActionCommand(
            @event.UserId,
            @event.ActionType, 
            @event.EntityName, 
            @event.EntityId, 
            @event.Success);
        
        await mediator.Send(command, context.CancellationToken);
    }
}