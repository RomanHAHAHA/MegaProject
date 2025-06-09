using Common.Domain.Models.Results;
using MediatR;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.GetPagedOrders;

public class GetPagedOrdersQueryHandler(
    IOrdersRepository ordersRepository) : IRequestHandler<GetPagedOrdersQuery, PagedList<OrderDto>>
{
    public async Task<PagedList<OrderDto>> Handle(GetPagedOrdersQuery request, CancellationToken cancellationToken)
    {
        var pagedOrders = await ordersRepository.GetPagedOrdersAsync(
            request.OrderFilter,
            request.SortParams,
            request.PageParams,
            cancellationToken);

        var dtos = pagedOrders.Items
            .Select(o => new OrderDto(o))
            .ToList();
        
        return new PagedList<OrderDto>(
            dtos, 
            pagedOrders.Page, 
            pagedOrders.PageSize,
            pagedOrders.TotalCount);
    }
}