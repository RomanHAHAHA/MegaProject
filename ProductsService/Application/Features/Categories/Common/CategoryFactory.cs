namespace ProductsService.Application.Features.Categories.Common;

public class CategoryFactory
{
    public global::ProductsService.Domain.Entities.Category ToEntity(CategoryCreateDto categoryCreateDto)
    {
        return new global::ProductsService.Domain.Entities.Category
        {
            Name = categoryCreateDto.Name,
            Description = categoryCreateDto.Description,
        };
    }
}