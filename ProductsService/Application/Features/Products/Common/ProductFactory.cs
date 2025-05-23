namespace ProductsService.Application.Features.Products.Common;

public class ProductFactory
{
    public global::ProductsService.Domain.Entities.Product MapToEntity(ProductCreateDto productCreateDto)
    {
        return new global::ProductsService.Domain.Entities.Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price ?? 0,
            StockQuantity = productCreateDto.StockQuantity ?? 0
        };
    } 
}