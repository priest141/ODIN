using Microsoft.EntityFrameworkCore;
using odin_Application.Interfaces;
using odin_Domain.Entities;
using odin_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace odin_Infrastructure.Repositories
{
    public class MeshtasticRepository(OsintDbContext context) : IMeshtasticRepository
    {
        public async Task<MeshtasticNode?> GetNodeByIdAsync(string nodeId) =>
            await context.MeshtasticNodes.FindAsync(nodeId);

        public async Task<IEnumerable<MeshtasticNode>> GetAllActiveNodesAsync(TimeSpan activeWithin)
        {
            var cutoff = DateTimeOffset.UtcNow.Subtract(activeWithin);

            return await context.MeshtasticNodes
                .Where(n => n.LastHeard >= cutoff)
                .ToListAsync();
        }

        public async Task UpsertNodeAsync(MeshtasticNode node)
        {
            var existing = await GetNodeByIdAsync(node.NodeId);
            if (existing == null)
            {
                await context.MeshtasticNodes.AddAsync(node);
            }
            else
            {
                // Updates the existing node with fresh telemetry (battery, GPS, voltage)
                context.Entry(existing).CurrentValues.SetValues(node);
            }

            await context.SaveChangesAsync();
        }

        public async Task AddMessageAsync(MeshtasticMessage message)
        {
            await context.MeshtasticMessages.AddAsync(message);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MeshtasticMessage>> GetRecentMessagesAsync(int count = 50) =>
            await context.MeshtasticMessages
                .Include(m => m.Sender) // EF Core fetches the node details automatically
                .OrderByDescending(m => m.Timestamp)
                .Take(count)
                .ToListAsync();

        public async Task<IEnumerable<MeshtasticMessage>> GetMessagesForNodeAsync(string nodeId, int count = 50) =>
                await context.MeshtasticMessages
                    .Where(m => m.SenderId == nodeId || m.ReceiverId == nodeId)
                    .OrderByDescending(m => m.Timestamp)
                    .Take(count)
                    .ToListAsync();
    }
}

