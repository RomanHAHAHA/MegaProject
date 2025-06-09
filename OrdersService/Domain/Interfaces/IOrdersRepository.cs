using Common.Domain.Dtos;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using OrdersService.Application.Features.GetPagedOrders;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Interfaces;

public interface IOrdersRepository : IRepository<Order, Guid>
{
    Task<PagedList<Order>> GetPagedOrdersAsync(
        OrderFilter orderFilter,
        SortParams sortParams,
        PageParams pageParams,
        CancellationToken cancellationToken);
    
    Task<List<UserOrderDto>> GetUserOrdersByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<OrderDto>> GetConfirmedOrdersAsync(CancellationToken cancellationToken = default);
    
    Task<bool> HasUserOrderedProductAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);
}