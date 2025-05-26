using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductCreatedEvent(Guid Id, string Name, decimal Price, int StockQuantity) : BaseEvent;