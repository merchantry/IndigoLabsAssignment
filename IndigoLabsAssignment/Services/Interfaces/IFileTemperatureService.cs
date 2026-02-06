using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Services.Interfaces
{
    public interface ICityTemperatureStatsService
    {
        Task<CityTemperatureStats?> GetSingleCityStatsAsync(string path, string city);

        Task<IEnumerable<CityTemperatureStats>> QueryCityStatsAsync(
            string path,
            double? minAverage,
            double? maxAverage,
            SortBy? sortBy = null,
            SortOrder? sortOrder = null
        );
    }
}
