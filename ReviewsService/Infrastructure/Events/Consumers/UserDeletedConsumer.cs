using Common.Infrastructure.Messaging.Events.User;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Users.Delete;

namespace ReviewsService.Infrastructure.Events.Consumers;

public class UserDeletedConsumer(IMediator mediator) : IConsumer<UserDeletedEvent>
{
    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var @event = context.Message;
        var command = new DeleteUserCommand(@event.UserId);
        await mediator.Send(command, context.CancellationToken);
    }
}