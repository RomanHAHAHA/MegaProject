using Common.Domain.Interfaces;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Interfaces;

public interface IOrdersRepository : IRepository<Order, Guid>
{
    Task<List<OrderDto>> GetUserOrdersByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    
    Task<OrderDto?> GetByIdWithItemsAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<bool> HasUserOrderedProductAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);
}