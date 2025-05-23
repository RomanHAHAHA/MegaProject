namespace Common.Domain.Interfaces;

public interface IEventConsumer<in TEvent>
    where TEvent : class
{
    Task ConsumeAsync(TEvent @event, CancellationToken cancellationToken);
}