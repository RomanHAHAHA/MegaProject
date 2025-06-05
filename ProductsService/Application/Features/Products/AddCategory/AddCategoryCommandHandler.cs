using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Products.AddCategory;

public class AddCategoryCommandHandler(
    IProductsRepository productsRepository,
    ICategoriesRepository categoriesRepository,
    IPublishEndpoint publisher,
    IHttpUserContext httpContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<AddCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        var category = await categoriesRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }
        
        product.Categories.Add(category);
        await OnCategoryAdded(category, product.Id, cancellationToken);
        
        var updated = await productsRepository.SaveChangesAsync(cancellationToken);
        
        return updated ?
            BaseResponse.Ok() : 
            BaseResponse.InternalServerError("Failed to add category");
    }

    private async Task OnCategoryAdded(
        Category category, 
        Guid productId,
        CancellationToken cancellationToken)
    {
        await publisher.Publish(new SystemActionEvent
        {
            CorrelationId = Guid.NewGuid(),
            SenderServiceName = serviceOptions.Value.Name,
            UserId = httpContext.UserId,
            ActionType = ActionType.Update,
            Message = $"Category \"{category.Name}\" added to product {productId}"
        }, cancellationToken);
    }
}