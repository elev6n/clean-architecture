using System.Text.Json;
using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArchitecture.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);
        return bytes == null ? default : JsonSerializer.Deserialize<T>(bytes);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken ct = default
    )
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        await _cache.SetAsync(key, bytes, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);
        return bytes != null;
    }
}
