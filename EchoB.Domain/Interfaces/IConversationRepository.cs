using EchoB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation> GetByUsersAsync(string user1Id, string user2Id);
        Task<Conversation> CreateAsync(Conversation conversation);
        Task<Conversation> UpdateAsync(Conversation conversation);
        Task<List<Conversation>> GetUserConversationsAsync(string userId, int skip, int take);
        Task<Conversation> GetByIdAsync(string conversationId);
        Task MarkConversationMessagesAsReadAsync(string conversationId, string userId, CancellationToken cancellationToken);


    }
}
