using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Delete;

public class DeleteCategoryCommandHandler(
    ICategoriesRepository categoriesRepository) : IRequestHandler<DeleteCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        DeleteCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var category = await categoriesRepository.GetByIdAsync(
            request.CategoryId, 
            cancellationToken);

        if (category is null)
        {
            return BaseResponse.NotFound(nameof(global::ProductsService.Domain.Entities.Category));
        }
        
        var deleted = await categoriesRepository.DeleteAsync(category, cancellationToken);
        
        return deleted ? 
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to delete category");
    }
}