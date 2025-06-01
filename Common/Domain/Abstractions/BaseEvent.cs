namespace Common.Domain.Abstractions;

public abstract record BaseEvent
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}