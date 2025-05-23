using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.AddCategory;

public class AddCategoryCommandHandler(
    IProductsRepository productsRepository,
    ICategoriesRepository categoriesRepository) : IRequestHandler<AddCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        AddCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        var category = await categoriesRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }
        
        product.Categories.Add(category);
        var updated = await productsRepository.UpdateAsync(product, cancellationToken);
        
        return updated ?
            BaseResponse.Ok() : 
            BaseResponse.InternalServerError("Failed to add category");
    }
}