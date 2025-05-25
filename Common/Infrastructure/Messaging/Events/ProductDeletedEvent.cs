using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductDeletedEvent(Guid ProductId) : BaseEvent;