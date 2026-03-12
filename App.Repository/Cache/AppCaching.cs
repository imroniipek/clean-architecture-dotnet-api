using Microsoft.Extensions.Caching.Memory;
namespace App.Repository.Cache;

public class AppCaching : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    //Burda verilerimi bir string key ile verip ordan cekmeye calsııyorum da diyebiliriz//
    public AppCaching(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string cacheKey)
    {
        _memoryCache.TryGetValue(cacheKey, out T? value);
        return Task.FromResult(value);
    }

    public Task AddAsync<T>(string cacheKey, T data, TimeSpan expiration)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        _memoryCache.Set(cacheKey, data, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
        return Task.CompletedTask;
    }
}