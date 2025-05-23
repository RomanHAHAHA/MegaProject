using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Application.Features.Products.Common;

namespace ProductsService.Application.Features.Products.Update;

public record UpdateProductCommand(
    Guid ProductId,
    ProductCreateDto ProductCreateDto) : IRequest<BaseResponse<Guid>>; 