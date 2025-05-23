using Common.Domain.Entities;

namespace Common.Domain.Interfaces;

public interface IOutboxMessagesRepository : IRepository<OutboxMessage, Guid>
{
    Task<List<OutboxMessage>> GetUnhandledMessagesAsync(
        int count,
        CancellationToken cancellationToken = default);
}