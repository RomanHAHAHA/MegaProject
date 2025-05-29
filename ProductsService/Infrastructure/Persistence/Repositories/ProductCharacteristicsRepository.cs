using Microsoft.EntityFrameworkCore;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Infrastructure.Persistence.Repositories;

public class ProductCharacteristicsRepository(ProductsDbContext dbContext) : IProductCharacteristicsRepository
{
    public async Task CreateAsync(
        ProductCharacteristic productCharacteristic,
        CancellationToken cancellationToken = default)
    {
        await dbContext.ProductCharacteristics.AddAsync(productCharacteristic, cancellationToken);
    }
    
    public async Task<ProductCharacteristic?> GetByIdAsync(
        Guid productId, 
        string name, 
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductCharacteristics
            .FirstOrDefaultAsync(pc => 
                pc.ProductId == productId && pc.Name == name, cancellationToken);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken) > 0;

    public void Delete(ProductCharacteristic characteristic)
    {
        dbContext.ProductCharacteristics.Remove(characteristic);
    }

    public void Update(ProductCharacteristic characteristic)
    {
        dbContext.ProductCharacteristics.Update(characteristic);
    }

    public async Task<List<ProductCharacteristic>> GetProductCharacteristicsAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductCharacteristics
            .AsNoTracking()
            .Where(pc => pc.ProductId == productId)
            .ToListAsync(cancellationToken);
    }
}