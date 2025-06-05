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

namespace ProductsService.Application.Features.Products.PutAwayCategory;

public class PutAwayCategoryCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publisher,
    IHttpUserContext httpContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<PutAwayCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(PutAwayCategoryCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdWithCategoriesAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        var categoryToPutAway = product.Categories.FirstOrDefault(c => c.Id == request.CategoryId);

        if (categoryToPutAway is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }
        
        product.Categories.Remove(categoryToPutAway);
        await OnCategoryRemoved(categoryToPutAway, product.Id, cancellationToken);
        
        var updated = await productsRepository.SaveChangesAsync(cancellationToken);

        return updated ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }
    
    private async Task OnCategoryRemoved(
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
            Message = $"Category \"{category.Name}\" removed product {productId}"
        }, cancellationToken);
    }
}