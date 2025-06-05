
using Common.Infrastructure.Messaging.Events;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using OrdersService.Application.Features.Orders.Confirm;

namespace OrdersService.Infrastructure.Messaging.Consumers;

public class ProductsReservedConsumer(IMediator mediator) : IConsumer<ProductsReservedEvent>
{
    public async Task Consume(ConsumeContext<ProductsReservedEvent> context)
    {
        var command = new ConfirmOrderProcessingCommand(
            context.Message.OrderId, 
            context.Message.UserId);
        
        await mediator.Send(command, context.CancellationToken);
    }
}