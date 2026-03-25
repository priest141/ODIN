using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using odin_Application.Interfaces;
using odin_Domain.Entities;
using odin_Domain.Enums;
using odin_Infrastructure.Data;

namespace odin_Infrastructure.Repositories;

public class OsintEventRepository(OsintDbContext context) : IOsintEventRepository
{
    public async Task AddEventAsync(OsintEvent osintEvent)
    {
        await context.OsintEvents.AddAsync(osintEvent);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OsintEvent>> GetActiveEventsAsync() =>
        await context.OsintEvents
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();

    public async Task<IEnumerable<OsintEvent>> GetEventsByCategoryAsync(string category) =>
        await context.OsintEvents
            .Where(e => e.EventCategory == category)
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();

    // The core OSINT spatial query
    public async Task<IEnumerable<OsintEvent>> GetEventsWithinRadiusAsync(Point center, double radiusInMeters) =>
        await context.OsintEvents
            .Where(e => e.Location != null && e.Location.IsWithinDistance(center, radiusInMeters))
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();

    public async Task<IEnumerable<OsintEvent>> GetEventsBySeverityWithinRadiusAsync(Point center, double radiusInMeters, EventSeverity minSeverity) =>
        await context.OsintEvents
            .Where(e => e.Severity >= minSeverity)
            .Where(e => e.Location != null && e.Location.IsWithinDistance(center, radiusInMeters))
            .Where(e => e.ExpiresAt == null || e.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();

    public async Task CleanupExpiredEventsAsync()
    {
        var expired = await context.OsintEvents
            .Where(e => e.ExpiresAt.HasValue && e.ExpiresAt < DateTimeOffset.UtcNow)
            .ToListAsync();

        if (expired.Count != 0)
        {
            context.OsintEvents.RemoveRange(expired);
            await context.SaveChangesAsync();
        }
    }
}
