using Common.Infrastructure.Messaging.Events.User;
using MassTransit;
using MediatR;
using UsersService.Application.Features.Users.Delete;

namespace UsersService.Infrastructure.Messaging.Consumers;

public class UserSnapshotCreationFailedConsumer(
    IMediator mediator) : IConsumer<UserSnapshotCreationFailedEvent>
{
    public async Task Consume(ConsumeContext<UserSnapshotCreationFailedEvent> context)
    {
        var @event = context.Message;
        var command = new DeleteUserCommand(@event.UserId);
        await mediator.Send(command, context.CancellationToken);
    }
}