using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Application.Features.Products.Common;

namespace ProductsService.Application.Features.Products.Create;

public record CreateProductCommand(
    ProductCreateDto ProductCreateDto,
    Guid UserId) : IRequest<BaseResponse<Guid>>;