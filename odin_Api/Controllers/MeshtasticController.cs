using Microsoft.AspNetCore.Mvc;
using odin_Application.Interfaces;

namespace odin_Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MeshtasticController(IMeshtasticRepository repository): ControllerBase
    {
        [HttpGet("nodes/active")]
        public async Task<IActionResult> GetActiveNodes([FromQuery] int activeWithinMinutes = 60)
        {
            var timespan = TimeSpan.FromMinutes(activeWithinMinutes);
            var nodes = await repository.GetAllActiveNodesAsync(timespan);

            var result = nodes.Select(n => new
            {
                n.NodeId,
                n.LongName,
                n.ShortName,
                n.BatteryLevel,
                n.Voltage,
                n.LastHeard,
                Latitude = n.Location?.Y,
                Longitude = n.Location?.X
            });

            return Ok(result);
        }

        [HttpGet("nodes/{nodeId}/messages")]
        public async Task<IActionResult> GetNodeMessages(string nodeId, [FromQuery] int count = 50)
        {
            var messages = await repository.GetMessagesForNodeAsync(nodeId, count);

            var result = messages.Select(m => new
            {
                m.Id,
                m.SenderId,
                SenderName = m.Sender?.LongName ?? "Unknown",
                m.ReceiverId,
                m.Payload,
                m.Snr,
                m.Timestamp
            });

            return Ok(result);
        }
    }
}

