using MediatR;

namespace ReviewsService.Application.Features.Reviews.GetPendingReviews;

public record GetPendingReviewsQuery : IRequest<List<PendingReviewDto>>;