using MediatR;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.GetUsersOrders;

public class GetUserOrdersCommandHandler(
    IOrdersRepository ordersRepository) : IRequestHandler<GetUserOrdersCommand, List<UserOrderDto>>
{
    public async Task<List<UserOrderDto>> Handle(GetUserOrdersCommand request, CancellationToken cancellationToken)
    {
        return await ordersRepository
            .GetUserOrdersByIdAsync(request.UserId, cancellationToken);
    }
}