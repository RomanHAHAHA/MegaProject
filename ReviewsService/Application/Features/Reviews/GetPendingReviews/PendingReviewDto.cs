using ReviewsService.Domain.Dtos;

namespace ReviewsService.Application.Features.Reviews.GetPendingReviews;

public class PendingReviewDto
{
    public required Guid ProductId { get; set; }
    
    public required UserReviewDto User { get; set; }

    public required string Text { get; set; } 

    public required double Rate { get; set; } 

    public required string CreatedAt { get; set; }
}