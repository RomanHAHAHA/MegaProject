using CartsService.Domain.Interfaces;
using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartsService.Infrastructure.Persistence.Repositories.Base;

public class ProductsRepository(CartsDbContext dbContext) : IProductRepository
{
    public async Task<bool> CreateAsync(
        ProductSnapshot product, 
        CancellationToken cancellationToken = default)
    {
        await dbContext.ProductSnapshots.AddAsync(product, cancellationToken); 
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<ProductSnapshot?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductSnapshots
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        ProductSnapshot product, 
        CancellationToken cancellationToken = default)
    {
        dbContext.ProductSnapshots.Update(product);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0; 
    }
}