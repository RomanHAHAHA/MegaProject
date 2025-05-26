using CartsService.Domain.Interfaces;
using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartsService.Infrastructure.Persistence.Repositories;

public class ProductsRepository(CartsDbContext dbContext) : IProductRepository
{
    public async Task CreateAsync(
        ProductSnapshot product, 
        CancellationToken cancellationToken = default)
    {
        await dbContext.ProductSnapshots.AddAsync(product, cancellationToken); 
    }

    public async Task<ProductSnapshot?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductSnapshots
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Delete(ProductSnapshot product) => dbContext.ProductSnapshots.Remove(product);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken) > 0;

    public async Task<int?> GetStockQuantityById(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.ProductSnapshots
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return product?.StockQuantity;
    }
}