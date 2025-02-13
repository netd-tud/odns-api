using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DatabaseCache
{
    public class DatabaseCaching : IDatabaseCacher
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;
        private readonly ILogger<DatabaseCaching> _logger;

        public DatabaseCaching(IMemoryCache cache, IConfiguration config, ILogger<DatabaseCaching> logger)
        {
            _memoryCache = cache;
            _config = config;
            _logger = logger;
        }

        public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getItemFunction)
        {
            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out T cachedItem))
                {
                    cachedItem = await getItemFunction();
                    // based on config set cache item timeout or not
                    bool isInConfig = Boolean.TryParse(_config.GetSection("Settings:Caching:UseTimeout").Value, out bool useTimeout);
                    if (isInConfig && useTimeout)
                    {
                        if (Int32.TryParse(_config.GetSection("Settings:Caching:TimeoutMinutes").Value, out int timeoutminutes))
                        {
                            _memoryCache.Set(cacheKey, cachedItem, TimeSpan.FromMinutes(timeoutminutes));
                            _logger.LogDebug($"Updated cache with new {typeof(T).FullName} on a {timeoutminutes} minutes TTL");
                            Console.WriteLine($"Updated cache with new {typeof(T).FullName} on a {timeoutminutes} minutes TTL");
                        }
                        else
                        {
                            throw new Exception("Configuration key Settings:Caching:TimeoutMinutes is missing or could not be parsed to Int32");
                        }

                    }
                    else
                    {
                        _memoryCache.Set(cacheKey, cachedItem); // set cache item and persist 
                        _logger.LogDebug($"Updated cache with new {typeof(T).FullName}");
                        Console.WriteLine($"Updated cache with new {typeof(T).FullName}");
                    }

                }
                return cachedItem;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Unable to update cache due to : " + ex.Message);
                return default(T);
            }

        }


        public async Task<T> GetOrSetAsync<T>(string cacheKey, int timeoutminutes, Func<Task<T>> getItemFunction)
        {
            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out T cachedItem))
                {
                    cachedItem = await getItemFunction();
                    // based on config set cache item timeout or not
                    //bool isInConfig = Boolean.TryParse(_config.GetSection("Settings:Caching:UseTimeout").Value, out bool useTimeout);
                    if (timeoutminutes > 0)
                    {

                        _memoryCache.Set(cacheKey, cachedItem, TimeSpan.FromMinutes(timeoutminutes));
                        _logger.LogDebug($"Updated cache with new {typeof(T).FullName} on a {timeoutminutes} minutes TTL");
                        Console.WriteLine($"Updated cache with new {typeof(T).FullName} on a {timeoutminutes} minutes TTL");


                    }
                    else
                    {
                        _memoryCache.Set(cacheKey, cachedItem); // set cache item and persist 
                        _logger.LogDebug($"Updated cache with new {typeof(T).FullName}");
                        Console.WriteLine($"Updated cache with new {typeof(T).FullName}");
                    }

                }
                return cachedItem;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Unable to update cache due to : " + ex.Message);
                return default(T);
            }

        }
    }

}
