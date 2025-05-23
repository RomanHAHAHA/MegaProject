using Common.Domain.Enums;
using Common.Domain.Extensions;
using Common.Domain.Interfaces;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Infrastructure.Persistence.Repositories.Logging;

public class LoggingOrdersRepository(
    IOrdersRepository ordersRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext) : IOrdersRepository  
{
    public async Task<bool> CreateAsync(
        Order entity, 
        CancellationToken cancellationToken = default)
    {
        var created = await ordersRepository.CreateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Create, entity.Id, created, cancellationToken);
        return created;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ordersRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var order = await ordersRepository.GetByIdAsync(id, cancellationToken);
        await OnActionPerformed(ActionType.Read, id, order is not null, cancellationToken);
        return order;
    }

    public async Task<bool> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await ordersRepository.ExistsAsync(id, cancellationToken); 
    }

    public async Task<bool> DeleteAsync(
        Order entity, 
        CancellationToken cancellationToken = default)
    {
        var deleted = await ordersRepository.DeleteAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Delete, entity.Id, deleted, cancellationToken);
        return deleted;
    }

    public async Task<bool> UpdateAsync(
        Order entity, 
        CancellationToken cancellationToken = default)
    {
        var updated = await ordersRepository.UpdateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Update, entity.Id, updated, cancellationToken);
        return updated;
    }

    public async Task<List<OrderDto>> GetUserOrdersByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await ordersRepository.GetUserOrdersByIdAsync(userId, cancellationToken);
    }

    public async Task<OrderDto?> GetByIdWithItemsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var order = await ordersRepository.GetByIdWithItemsAsync(id, cancellationToken);
        await OnActionPerformed(ActionType.Read, id, order is not null, cancellationToken);
        return order;
    }

    public async Task<bool> HasUserOrderedProductAsync(
        Guid userId, 
        Guid productId, 
        CancellationToken cancellationToken = default)
    {
        return await ordersRepository.HasUserOrderedProductAsync(
            userId,
            productId,
            cancellationToken); 
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
            nameof(Order),
            entityId, 
            success
        );

        await publishEndpoint.Publish(actionPerformedEvent, cancellationToken);
    }
}