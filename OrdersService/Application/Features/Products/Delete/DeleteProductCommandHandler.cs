﻿using MediatR;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Products.Delete;

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
            $"Deleted product with id: {request.ProductId}" :
            $"Failed to delete product with id: {request.ProductId}"; 
        
        logger.LogInformation(message);
    }
}