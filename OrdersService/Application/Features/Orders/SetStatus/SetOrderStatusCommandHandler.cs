using Common.Domain.Models.Results;
using MediatR;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.SetStatus;

public class SetOrderStatusCommandHandler(
    IOrdersRepository ordersRepository) : IRequestHandler<SetOrderStatusCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        SetOrderStatusCommand request, 
        CancellationToken cancellationToken)
    {
        var order = await ordersRepository
            .GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return BaseResponse.NotFound(nameof(Order));
        }
        
        order.Status = request.OrderStatus;
        
        await ordersRepository.UpdateAsync(order, cancellationToken);
        
        return BaseResponse.Ok();
    }
}