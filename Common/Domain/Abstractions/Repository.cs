using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Domain.Abstractions;

public abstract class Repository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey> 
    where TDbContext : DbContext
    where TEntity : Entity<TKey>
{
    protected readonly TDbContext AppDbContext;
    private readonly DbSet<TEntity> _dbSet;

    protected Repository(TDbContext appDbContext)
    {
        AppDbContext = appDbContext;
        _dbSet = AppDbContext.Set<TEntity>();
    }

    public virtual async Task<bool> CreateAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return await AppDbContext.SaveChangesAsync(cancellationToken) > 0; 
    }

    public virtual async Task<List<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsQueryable()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        TKey id, 
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext
            .Set<TEntity>()
            .AnyAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return await AppDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public virtual async Task<bool> UpdateAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return await AppDbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TKey id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
    }
}