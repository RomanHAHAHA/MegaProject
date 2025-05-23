namespace Common.Domain.Interfaces;

public interface ICacheService<T> where T : class
{
    Task SetAsync(
        string key,
        T data,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    Task<T?> GetAsync(string key, CancellationToken cancellationToken = default);

    Task<T?> GetAsync(
        string key, 
        Func<Task<T?>> factory, 
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}