using Common.Domain.Abstractions;

namespace ProductsService.Domain.Entities;

public sealed class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public double AverageRating { get; set; }

    public List<Category> Categories { get; set; } = [];

    public List<ProductImage> Images { get; set; } = [];
}