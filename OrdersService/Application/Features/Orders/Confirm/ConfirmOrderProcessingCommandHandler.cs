using Common.Application.Options;
using Common.Domain.Enums;
using Common.Infrastructure.Messaging.Events.Order;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.Confirm;

public class ConfirmOrderProcessingCommandHandler(
    IOrdersRepository ordersRepository,
    IPublishEndpoint publisher,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<ConfirmOrderProcessingCommand>
{
    public async Task Handle(ConfirmOrderProcessingCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            //TODO: handling error
            return;
        }

        try
        {
            order.Status = OrderStatus.Confirmed;
            await OnOrderProcessed(order.UserId, cancellationToken);
        
            await ordersRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            //TODO: handle error
            Console.WriteLine(e);
        }
    }

    private async Task OnOrderProcessed(Guid userId, CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new OrderProcessedEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                UserId = userId,
            }, 
            cancellationToken);
    }
}