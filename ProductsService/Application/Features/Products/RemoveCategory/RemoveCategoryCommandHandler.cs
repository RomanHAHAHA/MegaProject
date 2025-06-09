using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.RemoveCategory;

public class RemoveCategoryCommandHandler(
    IProductsRepository productsRepository) : IRequestHandler<RemoveCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository
            .GetByIdWithCategoriesAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }    
        
        product.Categories = product.Categories
            .Where(c => c.Id != request.CategoryId)
            .ToList();

        var updated = await productsRepository.SaveChangesAsync(cancellationToken);

        return updated ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }
}