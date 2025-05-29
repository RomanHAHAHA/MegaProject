using MediatR;

namespace ProductsService.Application.Features.Products.UpdateProductRating;

public record UpdateProductRatingCommand(Guid ProductId, double AverageRating) : IRequest;