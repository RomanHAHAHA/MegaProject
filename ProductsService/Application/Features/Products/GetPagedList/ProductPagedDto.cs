namespace ProductsService.Application.Features.Products.GetPagedList;

public class ProductPagedDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }

    public bool IsAvailable { get; init; }

    public double Rating { get; init; }

    public string MainImagePath { get; init; }

    public List<string> Categories { get; init; }

    public ProductPagedDto()
    {

    }
}