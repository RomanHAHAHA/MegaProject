using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductsReservedEvent(Guid OrderId, Guid UserId) : BaseEvent;