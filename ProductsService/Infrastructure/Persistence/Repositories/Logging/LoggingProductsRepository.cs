using Common.Domain.Dtos;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using ProductsService.Application.Features.Products.GetPagedList;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Infrastructure.Persistence.Repositories.Logging;

public class LoggingProductsRepository(
    IProductsRepository productsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext) : IProductsRepository
{
    public async Task<bool> CreateAsync(
        Product entity, 
        CancellationToken cancellationToken = default)
    {
        var created = await productsRepository.CreateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Create, entity.Id, created, cancellationToken);
        return created;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await productsRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetByIdAsync(id, cancellationToken);
        await OnActionPerformed(ActionType.Read, id, product is not null, cancellationToken);
        return product;
    }
    
    public async Task<bool> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await productsRepository.ExistsAsync(id, cancellationToken); 
    }

    public async Task<bool> DeleteAsync(
        Product entity, 
        CancellationToken cancellationToken = default)
    {
        var deleted = await productsRepository.DeleteAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Delete, entity.Id, deleted, cancellationToken);
        return deleted;
    }

    public async Task<bool> UpdateAsync(
        Product entity, 
        CancellationToken cancellationToken = default)
    {
        var updated = await productsRepository.UpdateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Update, entity.Id, updated, cancellationToken);
        return updated;
    }

    public async Task<PagedList<Product>> GetProductsAsync(
        ProductFilter productFilter, 
        SortParams sortParams, 
        PageParams pageParams,
        CancellationToken cancellationToken = default)
    {
        return await productsRepository.GetProductsAsync(
            productFilter, 
            sortParams, 
            pageParams, 
            cancellationToken); 
    }

    public async Task<Product?> GetAllInfoByIdAsync(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetAllInfoByIdAsync(productId, cancellationToken);
        await OnActionPerformed(ActionType.Read, productId, product is not null, cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdWithImagesAsync(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetByIdWithImagesAsync(productId, cancellationToken); 
        await OnActionPerformed(ActionType.Read, productId, product is not null, cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdWithCategories(
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        var product = await productsRepository.GetByIdWithCategories(productId, cancellationToken);
        await OnActionPerformed(ActionType.Read, productId, product is not null, cancellationToken);
        return product;
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
            nameof(Product), 
            entityId,
            success);

        await publishEndpoint.Publish(actionPerformedEvent, cancellationToken);
    }
}