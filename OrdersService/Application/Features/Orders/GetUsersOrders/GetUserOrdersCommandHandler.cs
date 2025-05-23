using MediatR;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.GetUsersOrders;

public class GetUserOrdersCommandHandler(
    IOrdersRepository ordersRepository) : IRequestHandler<GetUserOrdersCommand, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(
        GetUserOrdersCommand request, 
        CancellationToken cancellationToken)
    {
        return await ordersRepository
            .GetUserOrdersByIdAsync(request.UserId, cancellationToken);
    }
}