using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Products.Common;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Update;

public class UpdateProductCommandHandler(
    IProductsRepository productsRepository,
    IValidator<ProductCreateDto> validator,
    IPublishEndpoint publishEndpoint) : IRequestHandler<UpdateProductCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(
        UpdateProductCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request.ProductCreateDto);

        if (!validationResult.IsValid)
        {
            return BaseResponse<Guid>.BadRequest(validationResult);
        }
        
        var product = await productsRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse<Guid>.NotFound(nameof(Product));
        }
        
        UpdateProduct(product, request.ProductCreateDto);
        var updated = await productsRepository.UpdateAsync(product, cancellationToken);

        if (!updated)
        {
            return BaseResponse<Guid>.InternalServerError("Failed to update product");
        }

        await OnProductUpdated(product, cancellationToken);
        return BaseResponse<Guid>.Ok(product.Id);
    }

    private async Task OnProductUpdated(
        Product product,
        CancellationToken cancellationToken)
    {
        var productUpdatedEvent = new ProductUpdatedEvent(
            product.Id,
            product.Name,
            product.Price);

        await publishEndpoint.Publish(productUpdatedEvent, cancellationToken);
    }
    
    private void UpdateProduct(Product product, ProductCreateDto productCreateDto)
    {
        product.Name = productCreateDto.Name;
        product.Description = productCreateDto.Description;
        product.Price = productCreateDto.Price!.Value;
        product.StockQuantity = productCreateDto.StockQuantity!.Value;
    }
}