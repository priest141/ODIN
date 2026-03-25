using NetTopologySuite.Geometries;
using odin_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace odin_Domain.Entities
{
    public class OsintEvent
    {
        public int Id { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public EventSeverity Severity { get; set; } = EventSeverity.Info;
        public string? Description { get; set; }

        // Will be mapped to JSONB in Infrastructure
        public JsonDocument? RawData { get; set; }

        // Base Geometry allows for Points (accidents) or Polygons (wildfires)
        public Geometry? Location { get; set; }

        public DateTimeOffset DiscoveredAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ExpiresAt { get; set; }
    }
}

