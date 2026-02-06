using IndigoLabsAssignment.Config;
using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services.Cache.Interfaces;

namespace IndigoLabsAssignment.Services.Cache
{
    public class CityAggregateCacheService(ICacheService cacheService) : ICityAggregateCacheService
    {
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

        private readonly string _key = AppConstants.CityAggregatesCacheKey;

        private sealed class CacheEntry
        {
            public FileMetaData FileMetaData { get; init; }
            public required Dictionary<string, CityAggregate> Data { get; init; }
        }

        private bool CacheEntryExists()
        {
            return _cacheService.Exists(_key);
        }

        private CacheEntry GetCacheEntry()
        {
            if (!CacheEntryExists())
                throw new InvalidOperationException("Cache entry does not exist.");

            return _cacheService.Get<CacheEntry>(_key)!;
        }

        public bool ShouldRefresh(FileMetaData fileMetaData)
        {
            if (!CacheEntryExists())
                return true;

            var entry = GetCacheEntry();
            return !entry.FileMetaData.Equals(fileMetaData);
        }

        public Dictionary<string, CityAggregate> Get()
        {
            var entry = GetCacheEntry();
            return entry.Data;
        }

        public void Set(FileMetaData fileMetaData, Dictionary<string, CityAggregate> value)
        {
            _cacheService.Set(
                _key,
                new CacheEntry
                {
                    FileMetaData = fileMetaData,
                    Data = value
                },
                AppConstants.CityAggregatesCacheDuration
            );
        }

    }
}
