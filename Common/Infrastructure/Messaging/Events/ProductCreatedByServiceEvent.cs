using Common.Domain.Abstractions;
using Common.Domain.Enums;

namespace Common.Infrastructure.Messaging.Events;

public record ProductCreatedByServiceEvent(
    Guid CorrelationId,
    Guid ProductId,
    string UserId,
    string ServiceName) : BaseEvent;