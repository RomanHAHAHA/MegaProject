using Common.Domain.Entities;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.PutAwayCategory;

public class PutAwayCategoryCommandHandler(IProductsRepository productsRepository) : 
    IRequestHandler<PutAwayCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        PutAwayCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await productsRepository
            .GetByIdWithCategories(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        var categoryToPutAway = product.Categories.FirstOrDefault(c => c.Id == request.CategoryId);

        if (categoryToPutAway is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }
        
        product.Categories.Remove(categoryToPutAway);
        var updated = await productsRepository.UpdateAsync(product, cancellationToken);
        
        return updated ? 
            BaseResponse.Ok() : 
            BaseResponse.InternalServerError("Failed to update product categories");
    }
}