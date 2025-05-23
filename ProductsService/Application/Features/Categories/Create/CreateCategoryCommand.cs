using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Application.Features.Categories.Common;

namespace ProductsService.Application.Features.Categories.Create;

public record CreateCategoryCommand(CategoryCreateDto CategoryCreateDto) : IRequest<BaseResponse>;