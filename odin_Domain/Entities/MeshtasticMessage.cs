using System;
using System.Collections.Generic;
using System.Text;

namespace odin_Domain.Entities
{
    public class MeshtasticMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string? ReceiverId { get; set; }
        public string Payload { get; set; } = string.Empty;
        public decimal? Snr { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public MeshtasticNode? Sender {  get; set; }

    }
}

