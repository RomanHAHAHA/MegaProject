using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Infrastructure.Persistence.Repositories.Logging;

public class LoggingProductImagesRepository(
    IProductImagesRepository imagesRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext) : IProductImagesRepository
{
    public async Task<bool> CreateAsync(
        ProductImage entity, 
        CancellationToken cancellationToken = default)
    {
        var created = await imagesRepository.CreateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Create, entity.Id, created, cancellationToken);
        return created;
    }

    public async Task<List<ProductImage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await imagesRepository.GetAllAsync(cancellationToken); 
    }

    public async Task<ProductImage?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var image = await imagesRepository.GetByIdAsync(id, cancellationToken);
        await OnActionPerformed(ActionType.Read, id, image is not null, cancellationToken);
        return image;
    }

    public async Task<bool> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await imagesRepository.ExistsAsync(id, cancellationToken); 
    }
    
    public async Task<bool> DeleteAsync(
        ProductImage entity, 
        CancellationToken cancellationToken = default)
    {
        var deleted = await imagesRepository.DeleteAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Delete, entity.Id, deleted, cancellationToken);
        return deleted;
    }

    public async Task<bool> UpdateAsync(
        ProductImage entity, 
        CancellationToken cancellationToken = default)
    {
        var updated = await imagesRepository.UpdateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Update, entity.Id, updated, cancellationToken);
        return updated;
    }

    public async Task<List<ProductImage>> GetProductImagesAsync(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        return await imagesRepository.GetProductImagesAsync(productId, cancellationToken); 
    }
    
    private async Task OnActionPerformed(
        ActionType actionType,
        Guid entityId,
        bool success,
        CancellationToken cancellationToken)
    {
        var actionPerformedEvent = new DbActionPerformedEvent(
            httpUserContext.UserId,
            actionType,
            nameof(ProductImage), 
            entityId,
            success);

        await publishEndpoint.Publish(actionPerformedEvent, cancellationToken);
    }
}