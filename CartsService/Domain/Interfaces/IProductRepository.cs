using Common.Domain.Entities;

namespace CartsService.Domain.Interfaces;

public interface IProductRepository
{
    Task CreateAsync(ProductSnapshot product, CancellationToken cancellationToken = default);
    
    Task<ProductSnapshot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    void Delete(ProductSnapshot product);
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int?> GetStockQuantityById(
        Guid productId,
        CancellationToken cancellationToken = default);
}