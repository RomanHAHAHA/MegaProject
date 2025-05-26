using MediatR;

namespace ReviewsService.Application.Features.Reviews.GetProductReviews;

public record GetProductReviewsQuery(Guid ProductId) : IRequest<List<ProductReviewDto>>;