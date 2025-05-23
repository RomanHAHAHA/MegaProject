using Microsoft.EntityFrameworkCore;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Infrastructure.Persistence.Repositories.Base;

public class UsersRepository(ReviewsDbContext dbContext) : IUsersRepository
{
    public async Task<bool> CreateAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default)
    {
        await dbContext.UserSnapshots.AddAsync(user, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default)
    {
        dbContext.UserSnapshots.Update(user);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<UserSnapshot?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UserSnapshots
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UserSnapshots
            .AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default)
    {
        dbContext.UserSnapshots.Remove(user);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}