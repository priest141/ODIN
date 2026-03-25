using odin_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace odin_Application.Interfaces
{
    public interface IMeshtasticRepository
    {
        Task<MeshtasticNode?> GetNodeByIdAsync(string nodeId);
        Task<IEnumerable<MeshtasticNode>> GetAllActiveNodesAsync(TimeSpan activeWithin);
        Task UpsertNodeAsync (MeshtasticNode node);

        Task AddMessageAsync(MeshtasticMessage message);
        Task<IEnumerable<MeshtasticMessage>> GetRecentMessagesAsync(int count = 50);
        Task<IEnumerable<MeshtasticMessage>> GetMessagesForNodeAsync(string nodeId, int count = 50);
    }
}

