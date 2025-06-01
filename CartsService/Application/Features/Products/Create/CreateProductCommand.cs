using MediatR;

namespace CartsService.Application.Features.Products.Create;

public record CreateProductCommand(
    Guid CorrelationId,
    Guid Id,
    Guid SellerId,
    string Name,
    decimal Price) : IRequest;