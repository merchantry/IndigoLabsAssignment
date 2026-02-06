using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Services.Interfaces
{
    public interface ICityTemperatureStatsService
    {
        Task<CityTemperatureStats?> ComputeCityStatisticsAsync(string path, string city);

        Task<IEnumerable<CityTemperatureStats>> ComputeCitiesByAverageAsync(string path, double? minAverage, double? maxAverage, SortBy? sortBy = null, SortOrder? sortOrder = null);
    }
}
