using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductUpdatedEvent(
    Guid Id,
    string Name,
    decimal Price) : BaseEvent;
