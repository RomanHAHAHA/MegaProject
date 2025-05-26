using MediatR;

namespace OrdersService.Application.Features.Products.Create;

public record CreateProductCommand(
    Guid Id,
    string Name,
    decimal Price,
    int StockQuantity) : IRequest;