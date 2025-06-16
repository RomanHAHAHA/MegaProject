using MediatR;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Orders.GetPersonalOrders;

public class GetPersonalOrdersCommandHandler(
    IOrdersRepository ordersRepository) : IRequestHandler<GetPersonalOrdersCommand, List<PersonalOrderDto>>
{
    public async Task<List<PersonalOrderDto>> Handle(
        GetPersonalOrdersCommand request, 
        CancellationToken cancellationToken)
    {
        return await ordersRepository
            .GetPersonalOrdersAsync(request.UserId, cancellationToken);
    }
}