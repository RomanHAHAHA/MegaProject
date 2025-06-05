using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.SetStatus;

public class SetOrderStatusCommandHandler(
    IOrdersRepository ordersRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<SetOrderStatusCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(SetOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return BaseResponse.NotFound(nameof(Order));
        }
        
        order.Status = request.OrderStatus;
        await OnOrderStatusSet(order.Id, request.OrderStatus, cancellationToken);
        
        await ordersRepository.SaveChangesAsync(cancellationToken);
        
        return BaseResponse.Ok();
    }

    private async Task OnOrderStatusSet(
        Guid orderId, 
        OrderStatus orderStatus,
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new SystemActionEvent
        {
            CorrelationId = Guid.NewGuid(),
            SenderServiceName = serviceOptions.Value.Name,
            UserId = httpContext.UserId,
            ActionType = ActionType.Update,
            Message = $"Order {orderId} status set {orderStatus}"
        }, cancellationToken);
    }
}