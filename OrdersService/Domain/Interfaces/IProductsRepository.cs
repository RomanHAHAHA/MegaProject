using Common.Domain.Entities;

namespace OrdersService.Domain.Interfaces;

public interface IProductRepository
{
    Task<bool> CreateAsync(
        ProductSnapshot product, 
        CancellationToken cancellationToken = default);
    
    Task<ProductSnapshot?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default);
}