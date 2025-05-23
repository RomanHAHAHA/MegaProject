using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Domain.Enums;
using Common.Domain.Extensions;
using Common.Domain.Interfaces;
using Common.Infrastructure.Messaging.Events;
using MassTransit;

namespace CartsService.Infrastructure.Persistence.Repositories.Logging;

public class LoggingCartsRepository(
    ICartsRepository cartsRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext) : ICartsRepository
{
    public async Task<bool> CreateAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default)
    {
        var created = await cartsRepository.CreateAsync(cartItem, cancellationToken);
        await OnActionPerformed(ActionType.Create, cartItem.ProductId, created, cancellationToken);
        return created;
    }

    public async Task<bool> DeleteAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default)
    {
        var deleted = await cartsRepository.DeleteAsync(cartItem, cancellationToken);
        await OnActionPerformed(ActionType.Delete, cartItem.ProductId, deleted, cancellationToken);
        return deleted;
    }

    public async Task<bool> UpdateAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default)
    {
        var updated = await cartsRepository.UpdateAsync(cartItem, cancellationToken);
        await OnActionPerformed(ActionType.Update, cartItem.ProductId, updated, cancellationToken);
        return updated;
    }

    public async Task<CartItem?> GetByIdAsync(
        Guid userId, 
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        var cartItem = await cartsRepository.GetByIdAsync(userId, productId, cancellationToken);
        await OnActionPerformed(ActionType.Read, productId, cartItem is not null, cancellationToken);
        return cartItem;
    }

    public async Task<List<CartItem>> GetUserCartByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await cartsRepository.GetUserCartByIdAsync(userId, cancellationToken);
    }

    public async Task<bool> DeleteItemsFromUserCartAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await cartsRepository.DeleteItemsFromUserCartAsync(userId, cancellationToken);
    }
    
    private async Task OnActionPerformed(
        ActionType actionType,
        Guid productId,
        bool success,
        CancellationToken cancellationToken)
    {
        var entityId = httpUserContext.UserId.CombineGuids(productId);
        var actionPerformedEvent = new DbActionPerformedEvent(
            httpUserContext.UserId,
            actionType,
            nameof(CartItem),
            entityId, 
            success
        );

        await publishEndpoint.Publish(actionPerformedEvent, cancellationToken);
    }
}