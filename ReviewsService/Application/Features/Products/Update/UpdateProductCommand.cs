using MediatR;

namespace ReviewsService.Application.Features.Products.Update;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal Price) : IRequest;