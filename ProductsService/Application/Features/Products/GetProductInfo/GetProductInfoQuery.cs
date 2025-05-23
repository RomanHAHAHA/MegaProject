using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Products.GetProductInfo;

public record GetProductInfoQuery(Guid ProductId) : IRequest<BaseResponse<ProductInfoDto>>;