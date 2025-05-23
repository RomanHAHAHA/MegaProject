using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using CartsService.Infrastructure.Persistence.Repositories;
using CartsService.Infrastructure.Persistence.Repositories.Base;
using Common.Domain.Entities;
using MediatR;

namespace CartsService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
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