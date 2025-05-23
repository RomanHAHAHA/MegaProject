namespace Common.Domain.Abstractions;

public abstract record BaseEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}