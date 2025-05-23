using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;

namespace ProductsService.Application.Features.Categories.Delete;

public record DeleteCategoryCommand(Guid CategoryId) : IRequest<BaseResponse>;