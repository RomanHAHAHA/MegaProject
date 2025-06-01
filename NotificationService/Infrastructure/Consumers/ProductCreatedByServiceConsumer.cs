using Common.Infrastructure.Messaging.Events;
using MassTransit;

namespace NotificationService.Infrastructure.Consumers;

public class ProductCreatedByServiceConsumer :  IConsumer<ProductCreatedByServiceEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedByServiceEvent> context)
    {
        var @event = context.Message;
        return;
    }
}