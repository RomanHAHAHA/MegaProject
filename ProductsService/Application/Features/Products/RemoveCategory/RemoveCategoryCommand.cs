using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Products.RemoveCategory;

public record RemoveCategoryCommand(Guid ProductId, Guid CategoryId) : IRequest<BaseResponse>;