using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Application.Features.Products.Common;

namespace ProductsService.Application.Features.Products.Create;

public record CreateProductCommand(ProductCreateDto ProductCreateDto) : IRequest<BaseResponse<Guid>>;