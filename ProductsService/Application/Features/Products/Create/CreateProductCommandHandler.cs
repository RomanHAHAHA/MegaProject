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

namespace ProductsService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<CreateProductCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.FromCreateDto(request.ProductCreateDto, request.UserId);
        
        await productsRepository.CreateAsync(product, cancellationToken);
        await OnProductCreated(product, cancellationToken);
        
        var created = await productsRepository.SaveChangesAsync(cancellationToken);

        return created ? BaseResponse<Guid>.Ok(product.Id) : BaseResponse<Guid>.InternalServerError();
    }

    private async Task OnProductCreated(Product product, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var serviceName = serviceOptions.Value.Name;
        
        await publishEndpoint.Publish(new SystemActionEvent
        {
            CorrelationId = correlationId,
            SenderServiceName = serviceName,
            UserId = httpContext.UserId,
            ActionType = ActionType.Create,
            Message = $"Product {product.Id} created"
        }, cancellationToken);

        await publishEndpoint.Publish(
            new ProductCreatedEvent
            {
                CorrelationId = correlationId,
                SenderServiceName = serviceName,
                ProductId = product.Id,
                SellerId = product.UserId,
                Name = product.Name,
                Price = product.Price,
            }, 
            cancellationToken);
    }
}