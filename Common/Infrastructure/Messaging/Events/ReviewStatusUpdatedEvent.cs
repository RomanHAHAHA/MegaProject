using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ReviewStatusUpdatedEvent(Guid ProductId) : BaseEvent;