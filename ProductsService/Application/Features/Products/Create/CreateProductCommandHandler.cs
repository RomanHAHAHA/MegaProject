using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using ProductsService.Application.Features.Products.Common;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.Create;

public class CreateProductCommandHandler(
    IProductsRepository productsRepository,
    IValidator<ProductCreateDto> validator,
    ProductFactory productFactory,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateProductCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(
        CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request.ProductCreateDto);

        if (!validationResult.IsValid)
        {
            return BaseResponse<Guid>.BadRequest(validationResult);
        }
        
        var product = productFactory.MapToEntity(request.ProductCreateDto);
        var created = await productsRepository.CreateAsync(product, cancellationToken);

        if (!created)
        {
            return BaseResponse<Guid>.InternalServerError("Failed to create product");
        }

        await OnProductCreated(product, cancellationToken);
        return BaseResponse<Guid>.Ok(product.Id);
    }

    private async Task OnProductCreated(
        Product product, 
        CancellationToken cancellationToken)
    {
        var productCreatedEvent = new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.Price);
        
        await publishEndpoint.Publish(productCreatedEvent, cancellationToken);
    }
}