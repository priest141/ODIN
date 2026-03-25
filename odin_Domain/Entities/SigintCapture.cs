using odin_Domain.Enums;
using System.Text.Json;
using NetTopologySuite.Geometries;

namespace odin_Domain.Entities
{
    public class SigintCapture
    {
        public int Id { get; set; }
        public CaptureType Type { get; set; }
        public string Identifier { get; set; } = string.Empty;

        // Metadata like RSSI, SSID, or Flight Number
        public JsonDocument? Metadata { get; set; }

        public Point? CaptureLocation { get; set; }
        public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}

