using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Services
{
    public interface IFileTemperatureService
    {
        Task<CityTemperatureStats?> ComputeCityStatisticsAsync(string path, string city);

        Task<IEnumerable<CityTemperatureStats>> ComputeCitiesByAverageAsync(string path, double? minAverage, double? maxAverage, Models.SortBy? sortBy = null, Models.SortOrder? sortOrder = null);
    }
}
