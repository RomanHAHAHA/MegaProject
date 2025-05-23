namespace ProductsService.Application.Features.Products.Common;

public record ProductCreateDto(
    string Name,
    string Description,
    decimal? Price,
    int? StockQuantity);