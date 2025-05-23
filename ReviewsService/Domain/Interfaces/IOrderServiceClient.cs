using Common.Domain.Models.Results;

namespace ReviewsService.Domain.Interfaces;

public interface IOrderServiceClient
{
    Task<BaseResponse<bool>> HasUserOrderedProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}