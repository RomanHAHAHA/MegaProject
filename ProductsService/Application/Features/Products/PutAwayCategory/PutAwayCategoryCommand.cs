using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Products.PutAwayCategory;

public record PutAwayCategoryCommand(Guid ProductId, Guid CategoryId) : IRequest<BaseResponse>;