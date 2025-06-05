using Common.Application.Options;
using Common.Domain.Entities;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IPublishEndpoint publisher,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductSnapshot
        {
            Id = request.ProductId,
            Name = request.Name,
            Price = request.Price,
        };

        try
        {
            await productRepository.CreateAsync(product, cancellationToken);
            await OnProductCreated(request, cancellationToken);
            
            await productRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            await OnProductCreationFailed(request, cancellationToken);
            await productRepository.SaveChangesAsync(cancellationToken);
        }
    }
    
    private Task OnProductCreated(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return publisher.Publish(
            new ProductSnapshotCreatedEvent
            {
                CorrelationId = request.CorrelationId,
                SenderServiceName = serviceOptions.Value.Name,
                ProductId = request.ProductId,
                UserId = request.SellerId
            }, 
            cancellationToken);
    }
    
    private Task OnProductCreationFailed(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return publisher.Publish(
            new ProductSnapshotCreationFailedEvent
            {
                CorrelationId = request.CorrelationId,
                SenderServiceName = serviceOptions.Value.Name,
                ProductId = request.ProductId,
                UserId = request.SellerId,
            }, 
            cancellationToken);
    }
}