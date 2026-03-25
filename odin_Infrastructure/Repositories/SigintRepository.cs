using System.Linq;
using odin_Application.Interfaces;
using odin_Domain.Entities;
using odin_Domain.Enums;
using odin_Infrastructure.Data;
using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore;

namespace odin_Infrastructure.Repositories
{
    public class SigintRepository(OsintDbContext context): ISigintRepository
    {
        public async Task AddCaptureAsync(SigintCapture capture)
        {
            await context.SigintCaptures.AddAsync(capture);
            await context.SaveChangesAsync();
        }

        public async Task AddCaptureBulkAsync(IEnumerable<SigintCapture> captures)
        {
            await context.SigintCaptures.AddRangeAsync(captures);
            await context.SaveChangesAsync();

        }

        public async Task<IEnumerable<SigintCapture>> GetRecentCapturesByTypeAsync(CaptureType type, int count = 100) =>
            await context.SigintCaptures
                .Where(c => c.Type == type)
                .OrderByDescending(c => c.CapturedAt)
                .Take(count)
                .ToListAsync();

        public async Task<IEnumerable<SigintCapture>> GetCapturesByIdentifierAsync(string identifier) =>
                await context.SigintCaptures
                    .Where(c => c.Identifier == identifier)
                    .OrderByDescending(c => c.CapturedAt)
                    .ToListAsync();

        public async Task<IEnumerable<SigintCapture>> GetCapturesWithinRadiusAsync(Point center, double radiusInMeters) =>
            await context.SigintCaptures
                .Where(c => c.CaptureLocation != null && c.CaptureLocation.IsWithinDistance(center, radiusInMeters))
                .ToListAsync();
    }
}

