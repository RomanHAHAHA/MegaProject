using Common.Domain.Models.Results;

namespace CartsService.Domain.Interfaces;

public interface IProductsClient
{
    Task<BaseResponse<int>> GetProductQuantityAsync(
        Guid productId, 
        CancellationToken cancellationToken = default);
}