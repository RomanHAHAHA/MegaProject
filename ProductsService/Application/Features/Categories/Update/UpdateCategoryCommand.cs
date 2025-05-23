using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Application.Features.Categories.Common;

namespace ProductsService.Application.Features.Categories.Update;

public record UpdateCategoryCommand(
    Guid CategoryId,
    CategoryCreateDto CategoryCreateDto) : IRequest<BaseResponse>;