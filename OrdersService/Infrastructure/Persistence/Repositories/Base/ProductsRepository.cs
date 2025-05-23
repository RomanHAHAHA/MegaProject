using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Infrastructure.Persistence.Repositories.Base;

public class ProductsRepository(OrdersDbContext dbContext) : IProductRepository
{
    public async Task<bool> CreateAsync(
        ProductSnapshot product, 
        CancellationToken cancellationToken = default)
    {
        await dbContext.Product.AddAsync(product, cancellationToken); 
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<ProductSnapshot?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Product
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        ProductSnapshot product, 
        CancellationToken cancellationToken = default)
    {
        dbContext.Product.Update(product);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0; 
    }
}