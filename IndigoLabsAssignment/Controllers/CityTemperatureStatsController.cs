using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services.Interfaces;
using IndigoLabsAssignment.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IndigoLabsAssignment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityTemperatureStatsController(
        ICityTemperatureStatsService fileService,
        IOptions<FileSettings> fileSettings
    ) : ControllerBase
    {
        private readonly ICityTemperatureStatsService _fileService =
            fileService ?? throw new ArgumentNullException(nameof(fileService));

        private readonly string _filePath =
            fileSettings?.Value?.Path ?? throw new ArgumentNullException(nameof(fileSettings));

        /// <summary>
        /// Retrieves city temperature statistics with optional filtering and sorting.
        /// </summary>
        /// <param name="min">Minimum average temperature filter (optional)</param>
        /// <param name="max">Maximum average temperature filter (optional)</param>
        /// <param name="sortBy">Sort field: "city" or "avgtemp" (optional)</param>
        /// <param name="sortOrder">Sort direction: "asc" or "desc" (optional)</param>
        /// <returns>Filtered and sorted city statistics</returns>
        [HttpGet]
        public async Task<ActionResult<object>> Get(
            [FromQuery] double? min,
            [FromQuery] double? max,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder
        )
        {
            bool hasSortBy = !string.IsNullOrEmpty(sortBy);
            bool hasSortOrder = !string.IsNullOrEmpty(sortOrder);

            SortBy sortByEnum = default;
            SortOrder sortOrderEnum = default;

            if (hasSortBy && !EnumUtils.ToEnum(sortBy, out sortByEnum))
            {
                return BadRequest(
                    new
                    {
                        Message = $"Invalid sortBy value: {sortBy}. Value must be either avgTemp or city",
                    }
                );
            }

            if (hasSortOrder && !EnumUtils.ToEnum(sortOrder, out sortOrderEnum))
            {
                return BadRequest(
                    new
                    {
                        Message = $"Invalid sortOrder value: {sortOrder}. Value must be either asc or desc",
                    }
                );
            }

            var stats = await _fileService.QueryCityStatsAsync(
                _filePath,
                min,
                max,
                hasSortBy ? sortByEnum : null,
                hasSortOrder ? sortOrderEnum : null
            );

            return Ok(
                new
                {
                    Filter = new
                    {
                        min,
                        max,
                        sortBy,
                        sortOrder,
                    },
                    Results = stats,
                }
            );
        }

        /// <summary>
        /// Retrieves temperature statistics for a specific city.
        /// </summary>
        /// <param name="city">The name of the city</param>
        /// <returns>Temperature statistics for the specified city</returns>
        [HttpGet("{city}")]
        public async Task<ActionResult<object>> Get(string city)
        {
            var cityStats = await _fileService.GetSingleCityStatsAsync(_filePath, city);
            if (cityStats == null)
                return NotFound(new { City = city, Message = "City not found" });

            return Ok(cityStats);
        }
    }
}
