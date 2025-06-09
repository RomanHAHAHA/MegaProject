using MediatR;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.GetPendingReviews;

public class GetPendingReviewsQueryHandler(
    IReviewsRepository reviewsRepository) : IRequestHandler<GetPendingReviewsQuery, List<PendingReviewDto>>
{
    public async Task<List<PendingReviewDto>> Handle(
        GetPendingReviewsQuery request, 
        CancellationToken cancellationToken)
    {
        return await reviewsRepository.GetPendingReviewsAsync(cancellationToken);
    }
}