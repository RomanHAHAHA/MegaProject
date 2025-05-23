using Common.Domain.Abstractions;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using Common.Domain.Extensions;
using Common.Domain.Models.Results;
using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Features.Products.GetPagedList;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Infrastructure.Persistence.Repositories.Base;

public class ProductsRepository(ProductsDbContext dbContext) : 
    Repository<ProductsDbContext, Product, Guid>(dbContext),
    IProductsRepository
{
    public async Task<PagedList<Product>> GetProductsAsync(
        ProductFilter productFilter,
        SortParams sortParams,
        PageParams pageParams,
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .Filter(productFilter)
            .Sort(sortParams)
            .ToPagedAsync(pageParams, cancellationToken);
    }

    public async Task<Product?> GetAllInfoByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Categories)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<Product?> GetByIdWithImagesAsync(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<Product?> GetByIdWithCategories(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }
}