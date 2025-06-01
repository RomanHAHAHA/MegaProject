using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;

namespace CartsService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IPublishEndpoint publisher) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductSnapshot
        {
            Id = request.Id,
            SellerId = request.SellerId,
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
        var @event = new ProductCreatedByServiceEvent(
            request.CorrelationId,
            request.Id,
            request.SellerId.ToString(),
            ProductCreationRequiredServices.CartsService);

        return publisher.Publish(@event, cancellationToken);
    }
    
    private Task OnProductCreationFailed(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var @event = new ProductFailedToCreateByServiceEvent(
            request.CorrelationId,
            request.Id,
            request.SellerId.ToString(),
            ProductCreationRequiredServices.CartsService);

        return publisher.Publish(@event, cancellationToken);
    }
}