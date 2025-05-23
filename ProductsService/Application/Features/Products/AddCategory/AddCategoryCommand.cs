using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Products.AddCategory;

public record AddCategoryCommand(Guid ProductId, Guid CategoryId) : IRequest<BaseResponse>;