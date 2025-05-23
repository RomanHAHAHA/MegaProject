using ReviewsService.Domain.Entities;

namespace ReviewsService.Domain.Interfaces;

public interface IUsersRepository
{
    Task<bool> CreateAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default);

    Task<UserSnapshot?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        UserSnapshot user,
        CancellationToken cancellationToken = default);
}