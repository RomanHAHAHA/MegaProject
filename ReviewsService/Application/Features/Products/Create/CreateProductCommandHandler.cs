using MediatR;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductsRepository productRepository,
    ILogger<CreateProductCommandHandler> logger) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(
        CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        var product = new ProductSnapshot
        {
            Id = request.Id,
            Name = request.Name,
            Price = request.Price
        };
        
        var created = await productRepository.CreateAsync(product, cancellationToken);
        var message = created ? 
            $"Product with id {product.Id} created" : 
            $"Failed to create product with id {product.Id}";
        
        logger.LogInformation(message);
    }
}