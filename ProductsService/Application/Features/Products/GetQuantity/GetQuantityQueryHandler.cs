using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.GetQuantity;

public class GetQuantityQueryHandler(
    IProductsRepository productsRepository) : IRequestHandler<GetQuantityQuery, BaseResponse<int>>
{
    public async Task<BaseResponse<int>> Handle(GetQuantityQuery request, CancellationToken cancellationToken)
    {
        var productQuantity = await productsRepository
            .GetQuantityById(request.ProductId, cancellationToken);

        return productQuantity is null ? 
            BaseResponse<int>.NotFound(nameof(Product)) : 
            BaseResponse<int>.Ok(productQuantity.Value);
    }
}