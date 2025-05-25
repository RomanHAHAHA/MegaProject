using Common.Domain.Abstractions;
using Common.Domain.Enums;

namespace Common.Infrastructure.Messaging.Events;

public record SystemActionEvent : BaseEvent
{
    public Guid UserId { get; set; }

    public ActionType ActionType { get; set; }

    public string Message { get; set; } = string.Empty;
}