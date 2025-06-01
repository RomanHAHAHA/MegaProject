using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductCreatedEvent(
    Guid ProductId, 
    Guid SellerId, 
    string Name,
    decimal Price) : BaseEvent;