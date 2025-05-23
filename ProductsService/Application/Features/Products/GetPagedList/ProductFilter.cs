namespace ProductsService.Application.Features.Products.GetPagedList;

public record ProductFilter(
    string? Name,
    decimal? Price,
    bool? IsAvailable,
    double? Rating,
    ICollection<string>? Categories);