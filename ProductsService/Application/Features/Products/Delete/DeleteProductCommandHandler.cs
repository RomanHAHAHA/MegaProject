using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.Product;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Delete;

public class DeleteProductCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<DeleteProductCommand, BaseResponse>
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

        return deleted ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }

    private async Task OnProductDeleted(Guid productId, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var serviceName = serviceOptions.Value.Name;
        
        await publishEndpoint.Publish(new SystemActionEvent
        {
            CorrelationId = correlationId,
            SenderServiceName = serviceName,
            UserId = httpContext.UserId,
            ActionType = ActionType.Delete,
            Message = $"Product {productId} deleted"
        }, cancellationToken);
        
        await publishEndpoint.Publish(
            new ProductDeletedEvent
            {
                CorrelationId = correlationId,
                SenderServiceName = serviceName,
                ProductId = productId,
            }, 
            cancellationToken);
    }
}