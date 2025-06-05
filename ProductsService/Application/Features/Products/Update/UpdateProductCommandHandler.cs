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
using ProductsService.Domain.Extensions;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Update;

public class UpdateProductCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<UpdateProductCommand, BaseResponse<Guid>>
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

        return updated ? BaseResponse<Guid>.Ok(product.Id) : BaseResponse<Guid>.InternalServerError();
    }

    private async Task OnProductUpdated(Product product, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var serviceName = serviceOptions.Value.Name;
        
        await publishEndpoint.Publish(new SystemActionEvent
        {
            CorrelationId = correlationId,
            SenderServiceName = serviceName,
            UserId = httpContext.UserId,
            ActionType = ActionType.Update,
            Message = $"Product {product.Id} updated"
        }, cancellationToken);
        
        await publishEndpoint.Publish(
            new ProductUpdatedEvent
            {
                CorrelationId = correlationId,
                SenderServiceName = serviceName,
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            }, 
            cancellationToken);
    }
}