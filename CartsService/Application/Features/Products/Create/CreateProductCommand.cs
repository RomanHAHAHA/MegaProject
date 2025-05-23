using MediatR;

namespace CartsService.Application.Features.Products.Create;

public record CreateProductCommand(
    Guid Id,
    string Name,
    decimal Price) : IRequest;