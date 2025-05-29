using Common.Domain.Abstractions;
using Common.Domain.Dtos;

namespace Common.Infrastructure.Messaging.Events;

public record OrderCreatedEvent(Guid UserId, Guid OrderId, List<CartItemDto> CartItems) : BaseEvent;