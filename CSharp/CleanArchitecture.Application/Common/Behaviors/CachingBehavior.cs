using CleanArchitecture.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ICacheService _cacheService;

    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        ICacheService cacheService,
        ILogger<CachingBehavior<TRequest, TResponse>> logger
    )
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not ICacheable cacheableRequest)
            return await next();

        var cacheKey = cacheableRequest.CacheKey;
        var cachedResponse = await
            _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);
        var response = await next();

        await _cacheService.SetAsync(
            cacheKey,
            response,
            cacheableRequest.Expiration,
            cancellationToken
        );

        return response;
    }
}

public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}