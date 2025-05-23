using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record EmailConfirmedEvent(string Email) : BaseEvent;