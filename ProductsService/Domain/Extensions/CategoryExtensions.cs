using ProductsService.Application.Features.Categories.Common;
using ProductsService.Domain.Entities;

namespace ProductsService.Domain.Extensions;

public static class CategoryExtensions
{
    public static void UpdateFromCreateDto(this Category category, CategoryCreateDto categoryCreateDto)
    {
        category.Name = categoryCreateDto.Name;
        category.Description = categoryCreateDto.Description;
    }
}