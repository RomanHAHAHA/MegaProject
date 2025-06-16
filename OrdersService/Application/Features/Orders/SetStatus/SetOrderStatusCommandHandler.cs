using Common.Application.Options;
using Common.Domain.Enums;
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
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<SetOrderStatusCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(SetOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await ordersRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse.NotFound(nameof(Order));
        }
        
        order.ChangeStatus(request.OrderStatus);
        await OnOrderStatusSet(request, cancellationToken);
        
        await ordersRepository.SaveChangesAsync(cancellationToken);
        
        return ApiResponse.Ok();
    }

    private async Task OnOrderStatusSet(SetOrderStatusCommand request, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new SystemActionEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                UserId = request.InitiatorUserId,
                ActionType = ActionType.Update,
                Message = $"Order {request.OrderId} status set {request.OrderStatus}"
            }, 
            cancellationToken);
    }
}