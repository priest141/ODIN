using NetTopologySuite.Geometries;
using odin_Domain.Entities;
using odin_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace odin_Application.Interfaces
{
    public interface ISigintRepository
    {
        Task AddCaptureAsync(SigintCapture capture);
        Task AddCaptureBulkAsync(IEnumerable<SigintCapture> captures);

        // Useful for tracking specific MAC address over time accross diferent GPS pings
        Task<IEnumerable<SigintCapture>> GetCapturesByIdentifierAsync(string identifier);

        // "Show me all WiFi/BLE handshakes intercepted near this specific coordinate"
        Task<IEnumerable<SigintCapture>> GetCapturesWithinRadiusAsync(Point center, double radiusInMeters);

        Task<IEnumerable<SigintCapture>> GetRecentCapturesByTypeAsync(CaptureType type, int count = 100);
    }
}

