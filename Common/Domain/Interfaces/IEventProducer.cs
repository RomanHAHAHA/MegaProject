using Common.Domain.Abstractions;

namespace Common.Domain.Interfaces;

public interface IEventProducer<in TEvent> where TEvent : BaseEvent
{
    Task PublishAsync(TEvent @event, CancellationToken cancellationToken = default);
}
