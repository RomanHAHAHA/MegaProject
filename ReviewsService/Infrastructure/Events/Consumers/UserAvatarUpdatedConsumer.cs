using Common.Infrastructure.Messaging.Events;
using Common.Infrastructure.Messaging.Events.User;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Users.UpdateAvatar;

namespace ReviewsService.Infrastructure.Events.Consumers;

public class UserAvatarUpdatedConsumer(IMediator mediator) : IConsumer<UserAvatarUpdatedEvent>
{
    public async Task Consume(ConsumeContext<UserAvatarUpdatedEvent> context)
    {
        var @event = context.Message;
        var command = new UpdateUserAvatarCommand(@event.UserId, @event.AvatarPath);
        await mediator.Send(command, context.CancellationToken);
    }
}