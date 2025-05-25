using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Interfaces;

public interface IUsersRepository
{
    Task CreateAsync(UserSnapshot user,CancellationToken cancellationToken = default);

    Task<UserSnapshot?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}