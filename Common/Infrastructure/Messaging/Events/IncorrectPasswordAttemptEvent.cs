using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record IncorrectPasswordAttemptEvent(Guid UserId) : BaseEvent;