using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Categories.GetAll;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.GetProductInfo;

public class GetProductInfoQueryHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext) : IRequestHandler<GetProductInfoQuery, BaseResponse<ProductInfoDto>>
{
    public async Task<BaseResponse<ProductInfoDto>> Handle(
        GetProductInfoQuery request, 
        CancellationToken cancellationToken)
    {
        var product = await productsRepository
            .GetAllInfoByIdAsync(request.ProductId, cancellationToken);

        await OnProductRead(request.ProductId, product, cancellationToken);
        
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

    private async Task OnProductRead(
        Guid productId, 
        Product? product, 
        CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new SystemActionEvent
        {
            UserId = httpContext.UserId,
            ActionType = ActionType.Read,
            Message = product is null ? 
                $"Product {productId} not found" : 
                $"Product {productId} read"
        }, cancellationToken);
        
        await productsRepository.SaveChangesAsync(cancellationToken);
    }
}