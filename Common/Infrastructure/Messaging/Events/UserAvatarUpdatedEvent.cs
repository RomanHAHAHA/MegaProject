using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record UserAvatarUpdatedEvent(
    Guid UserId,
    string AvatarPath) : BaseEvent;