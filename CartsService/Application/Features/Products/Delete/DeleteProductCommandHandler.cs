using CartsService.Domain.Interfaces;
using MediatR;

namespace CartsService.Application.Features.Products.Delete;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ILogger<DeleteProductCommandHandler> logger) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            logger.LogInformation($"Product with id: {request.ProductId} was not found");
            return;
        }
        
        productRepository.Delete(product);
        var deleted  = await productRepository.SaveChangesAsync(cancellationToken);
        
        var message = deleted ? 
            $"Failed to delete product with id: {request.ProductId}" : 
            $"Deleted product with id: {request.ProductId}";
        
        logger.LogInformation(message);
    }
}