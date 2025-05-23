using ProductsService.Application.Features.Categories.GetAll;

namespace ProductsService.Application.Features.Products.GetProductInfo;

public class ProductInfoDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public bool IsAvailable { get; set; }

    public List<ShortImageDto> Images { get; set; } = [];

    public List<ShortCategoryDto> Categories { get; set; } = [];

}