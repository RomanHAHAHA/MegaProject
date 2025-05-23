using Common.Domain.Entities;
using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Application.Features.Categories.GetAll;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.GetProductInfo;

public class GetProductInfoQueryHandler(IProductsRepository productsRepository) : 
    IRequestHandler<GetProductInfoQuery, BaseResponse<ProductInfoDto>>
{
    public async Task<BaseResponse<ProductInfoDto>> Handle(
        GetProductInfoQuery request, 
        CancellationToken cancellationToken)
    {
        var product = await productsRepository
            .GetAllInfoByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse<ProductInfoDto>.NotFound(nameof(Product));
        }

        return new ProductInfoDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            IsAvailable = product.StockQuantity > 0,
            Categories = product.Categories
                .Select(c => new ShortCategoryDto(c.Id, c.Name))
                .ToList(),
            Images = product.Images
                .OrderByDescending(i => i.IsMain)
                .Select(i => new ShortImageDto(i.Id, i.ImagePath))
                .ToList()
        };
    }
}