using ProductsService.Domain.Entities;

namespace ProductsService.Domain.Interfaces;

public interface IProductCharacteristicsRepository
{
    Task CreateAsync(
        ProductCharacteristic productCharacteristic,
        CancellationToken cancellationToken = default);
    
    public Task<ProductCharacteristic?> GetByIdAsync(
        Guid productId,
        string name,
        CancellationToken cancellationToken = default);
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    void Delete(ProductCharacteristic characteristic);
    
    void Update(ProductCharacteristic characteristic);

    Task<List<ProductCharacteristic>> GetProductCharacteristicsAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}