using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace LibraryService.Caching
{
    public class CompositeCacheService : ILibraryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IRemoteCacheService _remoteCache;

        public CompositeCacheService(IMemoryCache cache, IRemoteCacheService remoteCache)
        {
            _cache = cache;
            _remoteCache = remoteCache;
        }

        public async Task<T> GetAsync<T>(object key)
        {
            T cached;
            var local = _cache.TryGetValue<T>(key, out cached);
            if (local)
                return cached;

            var data = await _remoteCache.GetAsync<T>(key.ToString());
            if (data != null)
            {
                _cache.Set(key, data);
                return data;
            }
            return default;
        }

        public async Task SetAsync<T>(object key, T value)
        {
            await _remoteCache.SetAsync(key.ToString(), value);
            _cache.Set(key, value);
        }

        public async Task RemoveAsync(object key)
        {
            await _remoteCache.RemoveAsync(key.ToString());
            _cache.Remove(key);
        }
    }
}