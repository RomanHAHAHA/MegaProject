using System.Collections.Concurrent;
using Common.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Application.Services;

public class CacheService<T>(
    IDistributedCache distributedCache,
    ILogger<CacheService<T>> logger) : ICacheService<T> where T : class
{
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();

    public async Task SetAsync(
        string key, 
        T data, 
        TimeSpan expiration,
        CancellationToken cancellationToken = default) 
    {
        var serializedData = JsonConvert.SerializeObject(
            data,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });

        await distributedCache.SetStringAsync(
            key,
            serializedData,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
            }, token: cancellationToken);

        CacheKeys.TryAdd(key, false); 
        logger.LogInformation($"Cache [{key}] set.");
    }

    public async Task<T?> GetAsync(
        string key, 
        CancellationToken cancellationToken = default)
    {
        var value = await distributedCache.GetStringAsync(key, token: cancellationToken);

        if (value is null)
        {
            logger.LogInformation($"Cache miss for key [{key}].");

            return null;
        }

        logger.LogInformation($"Cache return value with key [{key}].");
        
        return JsonConvert.DeserializeObject<T>(value);
    }

    public async Task<T?> GetAsync(
        string key, 
        Func<Task<T?>> factory, 
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await GetAsync(key, cancellationToken);

        if (cachedValue is not null)
        {
            return cachedValue;
        }

        var newValue = await factory();

        if (newValue is null)
        {
            return null;
        }
        
        await SetAsync(key, newValue, expiration, cancellationToken);

        return newValue;

    }

    public async Task RemoveAsync(
        string key, 
        CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
        CacheKeys.TryRemove(key, out bool _); 

        logger.LogInformation($"Cache [{key}] removed.");
    }

    public async Task RemoveByPrefixAsync(
        string prefix, 
        CancellationToken cancellationToken = default)
    {
        var tasks = CacheKeys.Keys
            .Where(k => k.StartsWith(prefix)) 
            .Select(k => RemoveAsync(k, cancellationToken)); 

        await Task.WhenAll(tasks); 
        logger.LogInformation($"All cache keys with prefix [{prefix}] removed.");
    }
}