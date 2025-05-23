using Common.Domain.Abstractions;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Persistence.Repositories;

public class OutboxMessagesRepository<TContext>(TContext dbContext) : 
    Repository<TContext, OutboxMessage, Guid>(dbContext),
    IOutboxMessagesRepository where TContext : DbContext
{
    public async Task<List<OutboxMessage>> GetUnhandledMessagesAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await AppDbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.CreatedAt)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}