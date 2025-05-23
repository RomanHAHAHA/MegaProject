using Common.Domain.Entities;
using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Dtos;

public class OrderItemDto(OrderItem orderItem)
{
    public ProductSnapshot Product { get; set; } = orderItem.Product!;

    public int Quantity { get; set; } = orderItem.Quantity;
}