using Common.Domain.Entities;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Delete;

public class DeleteProductCommandHandler(IProductsRepository productsRepository) : 
    IRequestHandler<DeleteProductCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        DeleteProductCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        var deleted = await productsRepository.DeleteAsync(product, cancellationToken);
        
        return deleted ?
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to delete product");
    }
}