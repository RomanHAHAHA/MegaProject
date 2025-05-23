using Common.Domain.Abstractions;
using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Infrastructure.Persistence.Repositories.Base;

public class ProductImagesRepository(ProductsDbContext dbContext) : 
    Repository<ProductsDbContext, ProductImage, Guid>(dbContext),
    IProductImagesRepository
{
    public async Task<List<ProductImage>> GetProductImagesAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.ProductImages
            .Where(p => p.ProductId == productId)
            .OrderBy(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}