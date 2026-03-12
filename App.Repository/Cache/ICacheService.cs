namespace App.Repository.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string cacheKey);
    
    Task AddAsync<T>(string cacheKey, T data,TimeSpan expiration);

    Task RemoveAsync(string cacheKey);
    
}