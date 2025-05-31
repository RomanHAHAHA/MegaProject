using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Users.UpdateAvatar;

namespace ProductsService.Infrastructure.Messaging.Consumers;

public class UserAvatarUpdatedConsumer(IMediator mediator) : IConsumer<UserAvatarUpdatedEvent>
{
    public async Task Consume(ConsumeContext<UserAvatarUpdatedEvent> context)
    {
        var @event = context.Message;
        var command = new UpdateUserAvatarCommand(@event.UserId, @event.AvatarPath);
        await mediator.Send(command, context.CancellationToken);
    }
}