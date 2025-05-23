using Microsoft.EntityFrameworkCore;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Infrastructure.Persistence.Repositories.Base;

public class UsersRepository(OrdersDbContext dbContext) : IUsersRepository
{
    public async Task<bool> CreateAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<UserSnapshot?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(user);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0; 
    }
}