using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services.Cache.Interfaces;
using IndigoLabsAssignment.Services.Interfaces;
using IndigoLabsAssignment.Utilities;

namespace IndigoLabsAssignment.Services
{
    public class CityTemperatureStatsService(
        ILineParser lineParser,
        IFileReaderService fileReader,
        ICityAggregateCacheService cacheService,
        IFileMetaDataService fileMetaDataService
    ) : ICityTemperatureStatsService
    {
        private static readonly SemaphoreSlim _cacheSemaphore = new(1, 1);

        private readonly ILineParser _lineParser =
            lineParser ?? throw new ArgumentNullException(nameof(lineParser));
        private readonly IFileReaderService _fileReader =
            fileReader ?? throw new ArgumentNullException(nameof(fileReader));
        private readonly ICityAggregateCacheService _cacheService =
            cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly IFileMetaDataService _fileMetaDataService =
            fileMetaDataService ?? throw new ArgumentNullException(nameof(fileMetaDataService));

        public async Task<CityTemperatureStats?> GetSingleCityStatsAsync(string path, string city)
        {
            var aggregates = await GetCityAggregatesAsync(path);

            if (!aggregates.TryGetValue(city, out var cityAggregate))
                return null;

            return new CityTemperatureStats(
                city,
                cityAggregate.Min,
                cityAggregate.Max,
                cityAggregate.Sum,
                cityAggregate.Count
            );
        }

        public async Task<IEnumerable<CityTemperatureStats>> QueryCityStatsAsync(
            string path,
            double? minAvgTemp,
            double? maxAvgTemp,
            SortBy? sortBy = null,
            SortOrder? sortOrder = null
        )
        {
            var allCityStats = await GetCityAggregatesEnumarableAsync(path);

            if (minAvgTemp.HasValue)
                allCityStats = allCityStats.Where(c => c.AvgTemp > minAvgTemp.Value);
            if (maxAvgTemp.HasValue)
                allCityStats = allCityStats.Where(c => c.AvgTemp < maxAvgTemp.Value);
            if (sortBy.HasValue && sortOrder.HasValue)
            {
                allCityStats = EnumerableUtils.ApplySort<CityTemperatureStats, object>(
                    allCityStats,
                    sortOrder.Value,
                    c =>
                        sortBy.Value switch
                        {
                            SortBy.City => c.City,
                            SortBy.AvgTemp => c.AvgTemp,
                            _ => throw new ArgumentOutOfRangeException(
                                nameof(sortBy),
                                sortBy,
                                "Invalid SortBy value"
                            ),
                        }
                );
            }

            return allCityStats;
        }

        private async Task<IEnumerable<CityTemperatureStats>> GetCityAggregatesEnumarableAsync(
            string path
        )
        {
            var cityAggregates = await GetCityAggregatesAsync(path);

            return cityAggregates.Select(cityAggregatePair => new CityTemperatureStats(
                cityAggregatePair.Key,
                cityAggregatePair.Value.Min,
                cityAggregatePair.Value.Max,
                cityAggregatePair.Value.Sum,
                cityAggregatePair.Value.Count
            ));
        }

        private async Task<Dictionary<string, CityAggregate>> GetCityAggregatesAsync(string path)
        {
            var fileMetaData = _fileMetaDataService.FromPath(path);

            if (!_cacheService.ShouldRefresh(fileMetaData))
                return _cacheService.Get();

            return await _cacheSemaphore.ExecuteWithLockAsync(async () =>
            {
                if (!_cacheService.ShouldRefresh(fileMetaData))
                    return _cacheService.Get();

                var cityAggregates = await GenerateCityAggregatesAsync(path).ConfigureAwait(false);

                _cacheService.Set(fileMetaData, cityAggregates);

                return cityAggregates;
            });
        }

        private async Task<Dictionary<string, CityAggregate>> GenerateCityAggregatesAsync(
            string path
        )
        {
            var cityAggregateDictionary = new Dictionary<string, CityAggregate>(
                StringComparer.OrdinalIgnoreCase
            );

            await foreach (var line in _fileReader.ReadLinesAsync(path))
            {
                if (
                    !_lineParser.TryParse(line, out string parsedCity, out double parsedTemperature)
                )
                    continue;

                if (!cityAggregateDictionary.TryGetValue(parsedCity, out var aggregate))
                {
                    cityAggregateDictionary[parsedCity] = new CityAggregate(parsedTemperature);
                }
                else
                {
                    aggregate.Min = Math.Min(aggregate.Min, parsedTemperature);
                    aggregate.Max = Math.Max(aggregate.Max, parsedTemperature);
                    aggregate.Sum += parsedTemperature;
                    aggregate.Count += 1;
                }
            }

            return cityAggregateDictionary;
        }
    }
}
