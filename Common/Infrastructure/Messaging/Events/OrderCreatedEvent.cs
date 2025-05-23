using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record OrderCreatedEvent(Guid UserId) : BaseEvent;