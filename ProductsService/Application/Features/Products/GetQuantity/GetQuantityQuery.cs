using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Products.GetQuantity;

public record GetQuantityQuery(Guid ProductId) : IRequest<BaseResponse<int>>;