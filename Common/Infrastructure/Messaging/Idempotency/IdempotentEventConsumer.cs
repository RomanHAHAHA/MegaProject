using Common.Domain.Abstractions;
using Common.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Messaging.Idempotency;

public abstract class IdempotentEventConsumer<TEvent>(
    IConsumer<TEvent> consumer,
    ICacheService<object> cacheService,
    ILogger<IdempotentEventConsumer<TEvent>> logger) : IConsumer<TEvent> where TEvent : BaseEvent
{
    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var key = $"idempotent:{typeof(TEvent).Name}:{context.Message.CorrelationId}";

        if (await cacheService.ExistsAsync(key))
        {
            logger.LogInformation($"Skipping processed message {key}.");
            return;
        }

        logger.LogInformation($"Processing message {key}.");
        
        await consumer.Consume(context);
        await cacheService.SetAsync(key, new object(), TimeSpan.FromHours(1));
    }
}