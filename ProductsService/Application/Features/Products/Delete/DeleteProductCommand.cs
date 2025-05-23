using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Products.Delete;

public record DeleteProductCommand(Guid ProductId) : IRequest<BaseResponse>;