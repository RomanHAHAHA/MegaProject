﻿using MediatR;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Products.SetMainImage;

public class SetMainProductImageCommandHandler(
    IProductsRepository productRepository,
    ILogger<SetMainProductImageCommandHandler> logger) : 
    IRequestHandler<SetMainProductImageCommand>
{
    public async Task Handle(SetMainProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            logger.LogInformation($"Product with id: {request.ProductId} does not exist");
            return;
        }
        
        product.MainImagePath = request.ImagePath;
        var updated = await productRepository.SaveChangesAsync(cancellationToken);

        var message = updated ? 
            $"Failed to update product with id: {request.ProductId}" : 
            $"Set main image to product with id: {request.ProductId}";
        
        logger.LogInformation(message);
    }
}