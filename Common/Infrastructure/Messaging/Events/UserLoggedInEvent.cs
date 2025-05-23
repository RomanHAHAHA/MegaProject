using MediatR;

namespace Common.Infrastructure.Messaging.Events;

public record UserLoggedInEvent(Guid UserId) : INotification;