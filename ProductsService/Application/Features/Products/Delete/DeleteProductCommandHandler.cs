using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Delete;

public class DeleteProductCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext) : IRequestHandler<DeleteProductCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        productsRepository.Delete(product);
        await OnProductDeleted(product.Id, cancellationToken);
        
        var deleted = await productsRepository.SaveChangesAsync(cancellationToken);
        
        return deleted ?
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to delete product");
    }

    private async Task OnProductDeleted(Guid productId, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new SystemActionEvent
        {
            UserId = httpContext.UserId,
            ActionType = ActionType.Delete,
            Message = $"Product {productId} deleted"
        }, cancellationToken);
        
        await publishEndpoint.Publish(new ProductDeletedEvent(productId), cancellationToken);
    }
}