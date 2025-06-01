using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductFailedToCreateByServiceEvent(
    Guid CorrelationId,
    Guid ProductId,
    string UserId,
    string ServiceName) : BaseEvent;