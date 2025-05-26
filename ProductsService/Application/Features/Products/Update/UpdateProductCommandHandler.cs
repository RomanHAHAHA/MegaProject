using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Extensions;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Update;

public class UpdateProductCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext) : IRequestHandler<UpdateProductCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse<Guid>.NotFound(nameof(Product));
        }

        product.UpdateFromCreateDto(request.ProductCreateDto);
        
        await OnProductUpdated(product, cancellationToken);
        
        var updated = await productsRepository.SaveChangesAsync(cancellationToken);

        return updated ? 
            BaseResponse<Guid>.Ok(product.Id) : 
            BaseResponse<Guid>.InternalServerError("Failed to update product");
    }

    private async Task OnProductUpdated(Product product, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new SystemActionEvent
        {
            UserId = httpContext.UserId,
            ActionType = ActionType.Update,
            Message = $"Product {product.Id} updated"
        }, cancellationToken);
        
        await publishEndpoint.Publish(
            new ProductUpdatedEvent(product.Id, product.Name, product.Price, product.StockQuantity), 
            cancellationToken);
    }
}