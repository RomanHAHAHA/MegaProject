using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record OrderProcessedEvent(Guid UserId) : BaseEvent;
