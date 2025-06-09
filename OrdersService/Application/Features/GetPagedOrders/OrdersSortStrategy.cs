using System.Linq.Expressions;
using Common.Domain.Interfaces;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Features.GetPagedOrders;

public class OrdersSortStrategy : ISortStrategy<Order>
{
    public Expression<Func<Order, object>> GetKeySelector(string? orderBy)
    {
        if (orderBy == nameof(Order.TotalPrice))
        {
            return o => o.OrderItems.Sum(oi => oi.Product!.Price * oi.Quantity);
        }
        
        return orderBy switch
        {
            nameof(Order.CreatedAt) => p => p.CreatedAt,
            nameof(Order.Status) => p => p.Status,
            _ => p => p.Id
        };       
    }
}