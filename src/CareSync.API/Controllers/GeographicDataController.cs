using CareSync.Application.Common.Geographics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeographicDataController : ControllerBase
{
    private readonly IPhilippineGeographicDataService _geographicService;
    private readonly ILogger<GeographicDataController> _logger;

    public GeographicDataController(IPhilippineGeographicDataService geographicService, ILogger<GeographicDataController> logger)
    {
        _geographicService = geographicService ?? throw new ArgumentNullException(nameof(geographicService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Gets all provinces for dropdown population
    /// </summary>
    [HttpGet("provinces")]
    [AllowAnonymous] // Allow anonymous access for frontend dropdowns
    [ProducesResponseType(typeof(IEnumerable<ProvinceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetProvinces()
    {
        // Disable caching for fresh data
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        _logger.LogInformation("API: GetProvinces called");
        var provinces = await _geographicService.GetProvincesAsync();
        _logger.LogInformation("API: Returning {Count} provinces", provinces?.Count() ?? 0);
        return Ok(provinces);
    }

    /// <summary>
    ///     Gets cities for a specific province by province code
    /// </summary>
    [HttpGet("provinces/{provinceCode}/cities")]
    [AllowAnonymous] // Allow anonymous access for cascading dropdowns
    [ProducesResponseType(typeof(IEnumerable<CityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCitiesByProvinceCode(string provinceCode)
    {
        // Disable caching for fresh data
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        _logger.LogInformation("API: GetCitiesByProvinceCode called with provinceCode: {ProvinceCode}", provinceCode);
        var cities = await _geographicService.GetCitiesByProvinceCodeAsync(provinceCode);
        _logger.LogInformation("API: Returning {Count} cities for province {ProvinceCode}", cities?.Count() ?? 0, provinceCode);
        return Ok(cities);
    }

    /// <summary>
    ///     Gets barangays for a specific city by city code
    /// </summary>
    [HttpGet("cities/{cityCode}/barangays")]
    [AllowAnonymous] // Allow anonymous access for cascading dropdowns
    [ProducesResponseType(typeof(IEnumerable<BarangayDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BarangayDto>>> GetBarangaysByCityCode(string cityCode)
    {
        // Disable caching for fresh data
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        _logger.LogInformation("API: GetBarangaysByCityCode called with cityCode: {CityCode}", cityCode);
        var barangays = await _geographicService.GetBarangaysByCityCodeAsync(cityCode);
        _logger.LogInformation("API: Returning {Count} barangays for city {CityCode}", barangays?.Count() ?? 0, cityCode);
        return Ok(barangays);
    }
}
