﻿using ReviewsService.Application.Features.Reviews.GetFilteredReviews;
using ReviewsService.Application.Features.Reviews.GetProductReviews;
using ReviewsService.Domain.Entities;

namespace ReviewsService.Domain.Interfaces;

public interface IReviewsRepository
{
    Task CreateAsync(Review review,CancellationToken cancellationToken = default);

    Task<Review?> GetByIdAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);

    void Delete(Review review);
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<List<ProductReviewDto>> GetProductReviewsAsync(
        Guid productId,
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    Task<List<ReviewToModerateDto>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);

    Task<double> GetAverageProductRatingAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}