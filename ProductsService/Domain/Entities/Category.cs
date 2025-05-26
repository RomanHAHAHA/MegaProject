using Common.Domain.Abstractions;
using ProductsService.Application.Features.Categories.Common;

namespace ProductsService.Domain.Entities;

public class Category : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];

    public static Category FromCreateDto(CategoryCreateDto categoryCreateDto)
    {
        return new Category
        {
            Name = categoryCreateDto.Name,
            Description = categoryCreateDto.Description,
        };
    }
}