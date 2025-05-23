using MediatR;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Products.Update;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ILogger<UpdateProductCommandHandler> logger) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            logger.LogInformation($"Product with id: {request.Id} was not found");
            return;
        }
        
        product.Name = request.Name;
        product.Price = request.Price;
        
        var updated  = await productRepository.UpdateAsync(product, cancellationToken);
        
        var message = updated ? 
            $"Failed to update product with id: {request.Id}" : 
            $"Updated product with id: {request.Id}";
        
        logger.LogInformation(message);
    }
}