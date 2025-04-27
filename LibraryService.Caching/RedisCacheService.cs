using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LibraryService.Caching
{
    public class RedisCacheService : IRemoteCacheService
    {
        private readonly IDistributedCache _redis;

        public RedisCacheService(IDistributedCache redis)
        {
            _redis = redis;
        }
        public async Task<T> GetAsync<T>(object key)
        {
            var data = await _redis.GetStringAsync(key.ToString());

            if (!string.IsNullOrEmpty(data))
            {
                T val = JsonConvert.DeserializeObject<T>(data);
                return val;
            }

            return default(T);
        }
        public async Task SetAsync<T>(object key, T value)
        {
            string serialized = JsonConvert.SerializeObject(value);
            await _redis.SetStringAsync(key.ToString(), serialized);
        }
        public async Task RemoveAsync(object key)
        {
            await _redis.RemoveAsync(key.ToString());
        }       

    }
}