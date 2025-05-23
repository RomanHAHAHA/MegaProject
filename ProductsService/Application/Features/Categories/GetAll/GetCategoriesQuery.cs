using MediatR;

namespace ProductsService.Application.Features.Categories.GetAll;

public record GetCategoriesQuery : IRequest<List<ShortCategoryDto>>;