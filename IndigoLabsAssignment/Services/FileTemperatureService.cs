using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Utilities;

namespace IndigoLabsAssignment.Services
{
    public class FileTemperatureService(ILineParser lineParser, IFileReaderService fileReader) : IFileTemperatureService
    {
        private readonly ILineParser _lineParser = lineParser ?? throw new ArgumentNullException(nameof(lineParser));
        private readonly IFileReaderService _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));

        public async Task<CityTemperatureStats?> ComputeCityStatisticsAsync(string path, string city)
        {
            var dict = await AggregatePerCityAsync(path);
            if (!dict.TryGetValue(city, out var agg))
                return null;

            return new CityTemperatureStats
            {
                City = city,
                Min = agg.Min,
                Max = agg.Max,
                AvgTemp = agg.Sum / agg.Count,
                Count = agg.Count
            };
        }

        public async Task<IEnumerable<CityTemperatureStats>> ComputeCitiesByAverageAsync(string path, double? minAvgTemp, double? maxAvgTemp, SortBy? sortBy = null, SortOrder? sortOrder = null)
        {
            var allCityStats = await ComputeAllCityStatisticsAsync(path);

            if (minAvgTemp.HasValue) allCityStats = allCityStats.Where(c => c.AvgTemp > minAvgTemp.Value);
            if (maxAvgTemp.HasValue) allCityStats = allCityStats.Where(c => c.AvgTemp < maxAvgTemp.Value);
            if (sortBy.HasValue && sortOrder.HasValue)
            {

                allCityStats = EnumerableUtils.ApplySort<CityTemperatureStats, object>(
                    allCityStats,
                    sortOrder.Value,
                    c => sortBy.Value switch
                    {
                        SortBy.City => c.City,
                        SortBy.AvgTemp => c.AvgTemp,
                        _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, "Invalid SortBy value")
                    }
                );
            }

            return allCityStats;
        }

        private async Task<IEnumerable<CityTemperatureStats>> ComputeAllCityStatisticsAsync(string path)
        {
            var dict = await AggregatePerCityAsync(path);

            return dict.Select(KvPair => new CityTemperatureStats
            {
                City = KvPair.Key,
                Min = KvPair.Value.Min,
                Max = KvPair.Value.Max,
                AvgTemp = KvPair.Value.Sum / KvPair.Value.Count,
                Count = KvPair.Value.Count
            });
        }

        private async Task<Dictionary<string, CityAggregate>> AggregatePerCityAsync(string path)
        {
            var dict = new Dictionary<string, CityAggregate>(StringComparer.OrdinalIgnoreCase);

            await foreach (var line in _fileReader.ReadLinesAsync(path))
            {
                if (!_lineParser.TryParse(line, out string parsedCity, out double temperature))
                    continue;

                if (!dict.TryGetValue(parsedCity, out var aggregate))
                {
                    dict[parsedCity] = new CityAggregate(temperature);
                }
                else
                {
                    aggregate.Min = Math.Min(aggregate.Min, temperature);
                    aggregate.Max = Math.Max(aggregate.Max, temperature);
                    aggregate.Sum += temperature;
                    aggregate.Count += 1;
                }
            }

            return dict;
        }
    }
}
