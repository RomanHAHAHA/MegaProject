using MediatR;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Products.SetMainImage;

public class SetMainProductImageCommandHandler(
    IProductRepository productRepository,
    ILogger<SetMainProductImageCommandHandler> logger) : 
    IRequestHandler<SetMainProductImageCommand>
{
    public async Task Handle(
        SetMainProductImageCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            logger.LogInformation($"Product with id: {request.ProductId} does not exist");
            return;
        }
        
        product.MainImagePath = request.ImagePath;
        var updated = await productRepository.UpdateAsync(product, cancellationToken);

        var message = updated ? 
            $"Failed to update product with id: {request.ProductId}" : 
            $"Set main image to product with id: {request.ProductId}";
        
        logger.LogInformation(message);
    }
}