using Common.Application.Options;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Products.SetMainImage;

public class SetMainProductImageCommandHandler(
    IProductRepository productRepository,
    IPublishEndpoint pubEndpoint,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<SetMainProductImageCommand>
{
    public async Task Handle(SetMainProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            //failed event
            return;
        }

        try
        {
            product.MainImagePath = request.ImagePath;
            //await OnMainImageSet(request, cancellationToken);
            
            var updated = await productRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /*private async Task OnMainImageSet(SetMainProductImageCommand request, CancellationToken cancellationToken)
    {
        await pubEndpoint.Publish(
            new ProductSnapshotMainImageSetEvent
            {
                CorrelationId = request.CorrelationId,
                SenderServiceName = serviceOptions.Value.Name,
                ProductId = request.ProductId,
            },
            cancellationToken);
    }*/
}