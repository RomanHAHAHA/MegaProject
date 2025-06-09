using MediatR;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.GetConfirmedOrders;

public class GetConfirmedOrdersQueryHandler(
    IOrdersRepository ordersRepository) : IRequestHandler<GetConfirmedOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(
        GetConfirmedOrdersQuery request, 
        CancellationToken cancellationToken)
    {
        return await ordersRepository.GetConfirmedOrdersAsync(cancellationToken);
    }
}