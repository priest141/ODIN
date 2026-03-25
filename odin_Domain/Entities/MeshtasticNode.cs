using System;
using System.Collections.Generic;
using System.Text;
using NetTopologySuite.Geometries;

namespace odin_Domain.Entities
{
    public class MeshtasticNode
    {
        public string NodeId { get; set; } = string.Empty;
        public string? LongName { get; set;  }
        public string? ShortName { get; set; }
        public string? MacAddress { get; set; }
        public int? BatteryLevel {  get; set; }
        public decimal? Voltage {  get; set; }
        public DateTimeOffset LastHeard { get; set; } = DateTimeOffset.UtcNow;

        public Point? Location { get; set; }

        public ICollection<MeshtasticMessage> Messages { get; set; } = new List<MeshtasticMessage>();
    }
}

