using Common.Domain.Entities;

namespace Common.Domain.Dtos;

public class CartItemDto
{
    public Guid UserId { get; set; }

    public ProductSnapshot Product { get; set; } = null!;

    public int Quantity { get; set; }
}