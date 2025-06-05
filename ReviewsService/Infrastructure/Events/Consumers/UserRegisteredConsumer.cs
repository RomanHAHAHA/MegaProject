using Common.Infrastructure.Messaging.Events.User;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Users.Create;

namespace ReviewsService.Infrastructure.Events.Consumers;

public class UserRegisteredConsumer(IMediator mediator) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var @event = context.Message;
        var command = new CreateUserCommand(
            @event.CorrelationId,
            @event.UserId, 
            @event.NickName,
            @event.ConnectionId);
        
        await mediator.Send(command, context.CancellationToken);
    }
}