using Common.Domain.Entities;

namespace Common.Domain.Dtos;

public class CartItemDto
{
    public ProductSnapshot Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal TotalPrice => Product.Price * Quantity;
}