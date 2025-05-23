using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Enums;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Infrastructure.Persistence.Repositories.Base;

public class OrdersRepository(OrdersDbContext dbContext) :
    Repository<OrdersDbContext, Order, Guid>(dbContext), 
    IOrdersRepository
{
    public async Task<List<OrderDto>> GetUserOrdersByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.DeliveryLocation)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .Select(o => new OrderDto(o))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<OrderDto?> GetByIdWithItemsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return null;
        /*return await AppDbContext.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.OrderItems)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Select(o => new OrderDto(o))
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);*/
    }

    public async Task<bool> HasUserOrderedProductAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Received)
            .SelectMany(o => o.OrderItems)
            .AnyAsync(oi => oi.ProductId == productId, cancellationToken);
    }
}