using Common.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.Hubs;
using StackExchange.Redis;

namespace NotificationService.Application.Features.Product.NotifyProductCreated;

public class NotifyProductCreatedCommandHandler(
    IHashCacheService hashCacheService,
    IRedisLockService redisLockService,
    IHubContext<NotificationHub, INotificationClient> hubContext) : IRequestHandler<NotifyProductSnapshotsCreatedCommand>
{
    private static readonly string[] RequiredServices =
    [
        ProductCreationRequiredServices.CartsService,
        ProductCreationRequiredServices.OrdersService,
        ProductCreationRequiredServices.ReviewsService
    ];

    public async Task Handle(NotifyProductSnapshotsCreatedCommand request, CancellationToken cancellationToken)
    {
        var lockKey = $"lock:product-created:{request.CorrelationId}";
        var gotLock = await redisLockService.AcquireLockWithRetryAsync(
            lockKey, 
            TimeSpan.FromSeconds(2), 
            maxRetries: 10, 
            delayBetweenRetries: TimeSpan.FromMilliseconds(50),
            cancellationToken);

        if (!gotLock)
        {
            return;
        }

        try
        {
            await hashCacheService.SetFieldAsync(
                request.CorrelationId.ToString(),
                request.SenderServiceName,
                TimeSpan.FromHours(1),
                cancellationToken);

            var successServices = await hashCacheService.GetAllFieldsAsync(
                request.CorrelationId.ToString(),
                cancellationToken);

            var allSucceeded = successServices.SetEquals(RequiredServices);
            
            if (allSucceeded)
            {
                await hubContext.Clients
                    .User(request.UserId.ToString())
                    .NotifyProductCreated(request.ProductId, "Product successfully created");

                await hashCacheService.RemoveAsync(request.CorrelationId.ToString());
            }
        }
        finally
        {
            await redisLockService.ReleaseLockAsync(lockKey);
        }
    }
}

public interface IHashCacheService
{
    Task SetFieldAsync(
        string key, 
        string field, 
        TimeSpan expiration, 
        CancellationToken token = default);
    
    Task<HashSet<string>> GetAllFieldsAsync(
        string key, 
        CancellationToken cancellationToken = default);
    
    Task RemoveAsync(string key);
}

public class RedisHashCacheService(IConnectionMultiplexer connection) : IHashCacheService
{
    private readonly IDatabase _db = connection.GetDatabase();

    public async Task SetFieldAsync(
        string key, 
        string field, 
        TimeSpan expiration, 
        CancellationToken token = default)
    {
        await _db.HashSetAsync(key, field, string.Empty);
        await _db.KeyExpireAsync(key, expiration); 
    }

    public async Task<HashSet<string>> GetAllFieldsAsync(string key, CancellationToken cancellationToken = default)
    {
        var entries = await _db.HashGetAllAsync(key);
        return entries.Select(e => e.Name.ToString()).ToHashSet();
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}

public interface IRedisLockService
{
    Task<bool> TryAcquireLockAsync(
        string key, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default);

    Task<bool> AcquireLockWithRetryAsync(
        string key,
        TimeSpan expiry,
        int maxRetries,
        TimeSpan delayBetweenRetries,
        CancellationToken cancellationToken = default);
    
    Task ReleaseLockAsync(string key);
}

public class RedisLockService(IConnectionMultiplexer redis) : IRedisLockService
{
    private readonly IDatabase _db = redis.GetDatabase();

    private readonly string _lockValue = Guid.NewGuid().ToString();

    public async Task<bool> TryAcquireLockAsync(
        string key, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default)
    {
        return await _db.StringSetAsync(key, _lockValue, expiry, When.NotExists);
    }
    
    public async Task<bool> AcquireLockWithRetryAsync(
        string key, 
        TimeSpan expiry, 
        int maxRetries, 
        TimeSpan delayBetweenRetries,
        CancellationToken cancellationToken = default)
    {   
        for (var i = 0; i < maxRetries; i++)
        {
            if (await TryAcquireLockAsync(key, expiry, cancellationToken))
            {
                return true;
            }
                
            await Task.Delay(delayBetweenRetries, cancellationToken);
        }

        return false;
    }

    public async Task ReleaseLockAsync(string key)
    {
        const string luaScript = @"
            if redis.call('GET', KEYS[1]) == ARGV[1] then
                return redis.call('DEL', KEYS[1])
            else
                return 0
            end";

        await _db.ScriptEvaluateAsync(luaScript, [key], [_lockValue]);
    }
}