using Common.Domain.Abstractions;

namespace Common.Infrastructure.Messaging.Events;

public record ProductMainImageSetEvent(Guid Id, string ImagePath) : BaseEvent;