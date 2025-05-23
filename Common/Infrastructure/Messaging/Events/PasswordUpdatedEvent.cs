using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record PasswordUpdatedEvent(Guid UserId) : BaseEvent;