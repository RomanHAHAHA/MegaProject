using Common.Domain.Models.Results;
using MediatR;

namespace OrdersService.Application.Features.HasUserOrderedProduct;

public record HasUserOrderedProductQuery(
    Guid UserId,
    Guid ProductId) : IRequest<BaseResponse<bool>>;