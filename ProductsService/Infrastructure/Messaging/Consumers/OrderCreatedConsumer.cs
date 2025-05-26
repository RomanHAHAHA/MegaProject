using Common.Infrastructure.Messaging.Events;
using MassTransit;

namespace ProductsService.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        
    }
}