using ReviewsService.Domain.Dtos;
using ReviewsService.Domain.Entities;

namespace ReviewsService.Domain.Interfaces;

public interface IReviewsRepository
{
    Task<bool> CreateAsync(
        Review review,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Review review,
        CancellationToken cancellationToken = default);

    Task<Review?> GetByIdAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Review review,
        CancellationToken cancellationToken = default);

    Task<List<ProductReviewDto>> GetProductReviewsAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<List<PendingReviewDto>> GetPendingReviewsAsync(
        CancellationToken cancellationToken = default);
}