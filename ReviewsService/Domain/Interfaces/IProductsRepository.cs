using ReviewsService.Domain.Entities;

namespace ReviewsService.Domain.Interfaces;

public interface IProductsRepository
{
    Task<bool> CreateAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default);

    Task<ProductSnapshot?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default);
}