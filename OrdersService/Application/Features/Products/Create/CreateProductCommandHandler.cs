using Common.Domain.Entities;
using MediatR;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ILogger<CreateProductCommandHandler> logger) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductSnapshot
        {
            Id = request.Id,
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
        };

        await productRepository.CreateAsync(product, cancellationToken);
        var created = await productRepository.SaveChangesAsync(cancellationToken);
        
        var message = created ? 
            $"Product with id {product.Id} created" : 
            $"Failed to create product with id {product.Id}";
        
        logger.LogInformation(message);
    }
}