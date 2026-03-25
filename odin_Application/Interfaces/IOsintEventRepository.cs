using NetTopologySuite.Geometries;
using odin_Domain.Entities;
using odin_Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace odin_Application.Interfaces
{
    public interface IOsintEventRepository
    {
        Task AddEventAsync(OsintEvent osintEvent);

        // Standard Filtering
        Task<IEnumerable<OsintEvent>> GetActiveEventsAsync();
        Task<IEnumerable<OsintEvent>> GetEventsByCategoryAsync(string category);

        // Spatial Filtering
        Task<IEnumerable<OsintEvent>> GetEventsWithinRadiusAsync(Point center, double radiusInMeters);
        Task<IEnumerable<OsintEvent>> GetEventsBySeverityWithinRadiusAsync(Point center, double radiusInMeters, EventSeverity minSeverity);

        // Maintenance
        Task CleanupExpiredEventsAsync();
    }
}

