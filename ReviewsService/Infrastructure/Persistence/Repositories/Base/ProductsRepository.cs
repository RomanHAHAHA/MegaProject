using Microsoft.EntityFrameworkCore;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Infrastructure.Persistence.Repositories.Base;

public class ProductsRepository(ReviewsDbContext dbContext) : IProductsRepository
{
    public async Task<bool> CreateAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default)
    {
        await dbContext.ProductSnapshots.AddAsync(product, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default)
    {
        dbContext.ProductSnapshots.Update(product);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<ProductSnapshot?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductSnapshots
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.ProductSnapshots
            .AnyAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        ProductSnapshot product,
        CancellationToken cancellationToken = default)
    {
        dbContext.ProductSnapshots.Remove(product);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}