using Microsoft.Extensions.Caching.Memory;
using IndigoLabsAssignment.Services.Cache.Interfaces;

namespace IndigoLabsAssignment.Services.Cache
{
    internal class CacheService(IMemoryCache cache) : ICacheService
    {
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (absoluteExpirationRelativeToNow.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            }

            _cache.Set(key, value, options);
        }

        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T? value) ? value : default;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
