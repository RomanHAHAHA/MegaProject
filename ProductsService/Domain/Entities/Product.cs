using Common.Domain.Abstractions;
using ProductsService.Application.Features.Products.Common;

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

    public List<ProductCharacteristic> Characteristics { get; set; } = [];
    
    public static Product FromCreateDto(ProductCreateDto createDto)
    {
        return new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price!.Value,
            StockQuantity = createDto.StockQuantity!.Value
        };
    }
}