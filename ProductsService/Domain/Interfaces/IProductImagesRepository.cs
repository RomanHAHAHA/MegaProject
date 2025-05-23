using Common.Domain.Entities;
using Common.Domain.Interfaces;
using ProductsService.Domain.Entities;

namespace ProductsService.Domain.Interfaces;

public interface IProductImagesRepository : IRepository<ProductImage, Guid>
{
    Task<List<ProductImage>> GetProductImagesAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}