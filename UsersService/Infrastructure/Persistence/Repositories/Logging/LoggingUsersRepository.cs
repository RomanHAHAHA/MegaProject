using Common.Domain.Dtos;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using UsersService.Application.Features.Users.GetPagedList;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Infrastructure.Persistence.Repositories.Logging;

public class LoggingUsersRepository(
    IUsersRepository usersRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext) : IUsersRepository
{
    public async Task<bool> CreateAsync(
        User entity, 
        CancellationToken cancellationToken = default)
    {
        var created = await usersRepository.CreateAsync(entity, cancellationToken); 
        await OnActionPerformed(ActionType.Create, entity.Id, created, cancellationToken);
        return created;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await usersRepository.GetAllAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var user = await usersRepository.GetByIdAsync(id, cancellationToken);
        await OnActionPerformed(ActionType.Read, id, user is not null, cancellationToken);
        return user;
    }

    public async Task<bool> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await usersRepository.ExistsAsync(id, cancellationToken); 
    }

    public async Task<bool> DeleteAsync(
        User entity, 
        CancellationToken cancellationToken = default)
    {
        var deleted = await usersRepository.DeleteAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Delete, entity.Id, deleted, cancellationToken);
        return deleted;
    }

    public async Task<bool> UpdateAsync(
        User entity, 
        CancellationToken cancellationToken = default)
    {
        var updated = await usersRepository.UpdateAsync(entity, cancellationToken);
        await OnActionPerformed(ActionType.Update, entity.Id, updated, cancellationToken);
        return updated;
    }

    public async Task<User?> GetByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        return await usersRepository.GetByEmailAsync(email, cancellationToken); 
    }

    public async Task<PagedList<User>> GetPagedList(
        UsersFilter usersFilter, 
        SortParams sortParams, 
        PageParams pageParams,
        CancellationToken cancellationToken = default)
    {
        return await usersRepository.GetPagedList(
            usersFilter,
            sortParams,
            pageParams,
            cancellationToken); 
    }

    public async Task<HashSet<PermissionEnum>> GetPermissionsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await usersRepository.GetPermissionsAsync(userId, cancellationToken);
    }

    public async Task<string?> GetRoleNameAsync(
        User user, 
        CancellationToken cancellationToken = default)
    {
        return await usersRepository.GetRoleNameAsync(user, cancellationToken); 
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
            nameof(User), 
            entityId,
            success);

        await publishEndpoint.Publish(actionPerformedEvent, cancellationToken);
    }
}