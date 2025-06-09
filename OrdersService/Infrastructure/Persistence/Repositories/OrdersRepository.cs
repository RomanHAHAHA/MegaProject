using Common.Domain.Abstractions;
using Common.Domain.Dtos;
using Common.Domain.Enums;
using Common.Domain.Extensions;
using Common.Domain.Models.Results;
using Microsoft.EntityFrameworkCore;
using OrdersService.Application.Features.GetPagedOrders;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Infrastructure.Persistence.Repositories;

public class OrdersRepository(OrdersDbContext dbContext) :
    Repository<OrdersDbContext, Order, Guid>(dbContext), 
    IOrdersRepository
{
    public async Task<PagedList<Order>> GetPagedOrdersAsync(
        OrderFilter orderFilter,
        SortParams sortParams,
        PageParams pageParams,
        CancellationToken cancellationToken)
    {
        return await AppDbContext.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Filter(orderFilter)
            .Sort(sortParams)
            .ToPagedAsync(pageParams, cancellationToken);
    }
    
    public async Task<List<UserOrderDto>> GetUserOrdersByIdAsync(
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
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new UserOrderDto(o))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<OrderDto>> GetConfirmedOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.DeliveryLocation)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == OrderStatus.Confirmed)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(o))
            .ToListAsync(cancellationToken);
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