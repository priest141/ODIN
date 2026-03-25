using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using odin_Application.Interfaces;
using odin_Domain.Enums;

namespace odin_Api.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class SigintController(ISigintRepository repository): ControllerBase
    {
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentCaptures([FromQuery] CaptureType type, [FromQuery] int count = 100)
        {
            var captures = await repository.GetRecentCapturesByTypeAsync(type, count);

            var result = captures.Select(c => new
            {
                c.Id,
                Type = c.Type.ToString(),
                c.Identifier,
                c.Metadata,
                Latitude = c.CaptureLocation?.Y,
                Longitude = c.CaptureLocation?.X,
                c.CapturedAt
            });

            return Ok(result);
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyCaptures(
            [FromQuery] double lat,
            [FromQuery] double lon,
            [FromQuery] double radiusInMeters = 10000)
        {
            var centerPoint = new Point(lon, lat) { SRID = 4326 };
            var captures = await repository.GetCapturesWithinRadiusAsync(centerPoint, radiusInMeters);

            var result = captures.Select(c => new
            {
                c.Id,
                Type = c.Type.ToString(),
                c.Identifier,
                c.Metadata,
                Latitude = c.CaptureLocation?.Y,
                Longitude = c.CaptureLocation?.X,
                c.CapturedAt
            });

            return Ok(result);

        }


    }
}

