using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record UserRegisteredEvent(
    Guid UserId,
    string NickName,
    string Email) : BaseEvent;