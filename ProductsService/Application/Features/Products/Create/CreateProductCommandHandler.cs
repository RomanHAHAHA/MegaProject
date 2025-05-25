using Common.Domain.Enums;
using Common.Domain.Interfaces;
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
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext) : IRequestHandler<CreateProductCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request.ProductCreateDto);

        if (!validationResult.IsValid)
        {
            return BaseResponse<Guid>.BadRequest(validationResult);
        }
        
        var product = productFactory.MapToEntity(request.ProductCreateDto);
        
        await productsRepository.CreateAsync(product, cancellationToken);
        await OnProductCreated(product, cancellationToken);
        
        var created = await productsRepository.SaveChangesAsync(cancellationToken);
        
        return created ?
            BaseResponse<Guid>.Ok(product.Id) :
            BaseResponse<Guid>.InternalServerError("Failed to create product");
    }

    private async Task OnProductCreated(Product product, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new SystemActionEvent
        {
            UserId = httpContext.UserId,
            ActionType = ActionType.Create,
            Message = $"Product {product.Id} created"
        }, cancellationToken);
        
        await publishEndpoint.Publish(
            new ProductCreatedEvent(product.Id, product.Name, product.Price), 
            cancellationToken);
    }
}