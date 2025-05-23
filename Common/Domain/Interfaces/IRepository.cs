using Common.Domain.Abstractions;

namespace Common.Domain.Interfaces;

public interface IRepository<TEntity, in TKey> where TEntity : Entity<TKey>
{
    Task<bool> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}