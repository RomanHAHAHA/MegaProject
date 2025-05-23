using Common.Domain.Abstractions;
using Common.Domain.Enums;

namespace Common.Infrastructure.Messaging.Events;

public record DbActionPerformedEvent(
    Guid UserId,
    ActionType ActionType, 
    string EntityName,
    Guid EntityId,
    bool Success) : BaseEvent;