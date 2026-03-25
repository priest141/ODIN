using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using odin_Application.Interfaces;
using odin_Domain.Enums;

namespace OsintDashboard.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OsintController(IOsintEventRepository repository) : ControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveEvents()
    {
        var events = await repository.GetActiveEventsAsync();

        var result = events.Select(e => new
        {
            e.Id,
            e.SourceName,
            e.EventCategory,
            Severity = e.Severity.ToString(),
            e.Description,
            e.RawData,
            // Cast to Point safely to extract the coordinates for the frontend map
            Latitude = (e.Location as Point)?.Y,
            Longitude = (e.Location as Point)?.X,
            e.DiscoveredAt,
            e.ExpiresAt
        });

        return Ok(result);
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearbyEvents(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] double radiusInMeters = 50000,
        [FromQuery] EventSeverity minSeverity = EventSeverity.Info)
    {
        var centerPoint = new Point(lon, lat) { SRID = 4326 };
        var events = await repository.GetEventsBySeverityWithinRadiusAsync(centerPoint, radiusInMeters, minSeverity);

        var result = events.Select(e => new
        {
            e.Id,
            e.SourceName,
            e.EventCategory,
            Severity = e.Severity.ToString(),
            e.Description,
            e.RawData,
            Latitude = (e.Location as Point)?.Y,
            Longitude = (e.Location as Point)?.X,
            e.DiscoveredAt,
            e.ExpiresAt
        });

        return Ok(result);
    }
}
