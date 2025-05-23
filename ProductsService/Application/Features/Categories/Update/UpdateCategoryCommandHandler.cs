using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Features.Categories.Common;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Update;

public class UpdateCategoryCommandHandler(
    ICategoriesRepository categoriesRepository) : IRequestHandler<UpdateCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        UpdateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var category = await categoriesRepository.GetByIdAsync(
            request.CategoryId, 
            cancellationToken);

        if (category is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }
        
        UpdateCategory(category, request.CategoryCreateDto);

        try
        {
            var updated = await categoriesRepository.UpdateAsync(category, cancellationToken);

            return updated ? 
                BaseResponse.Ok() :
                BaseResponse.InternalServerError("Failed to create category");
        }
        catch (DbUpdateException exception) when 
            (exception.InnerException is SqlException { Number: 2601 })
        {
            return BaseResponse.Conflict("Category with the same name already exists.");
        }
        catch (Exception)
        {
            return BaseResponse.InternalServerError();
        }
    }

    public void UpdateCategory(Category category, CategoryCreateDto categoryCreateDto)
    {
        category.Name = categoryCreateDto.Name;
        category.Description = categoryCreateDto.Description;
    }
}