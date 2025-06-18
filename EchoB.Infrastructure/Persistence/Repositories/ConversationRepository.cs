using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Persistence.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly EchoBDbContext _context;

        public ConversationRepository(EchoBDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation> GetByUsersAsync(string user1Id, string user2Id)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => (c.User1Id == Guid.Parse(user1Id) && c.User2Id == Guid.Parse(user2Id)) ||
                                         (c.User1Id == Guid.Parse(user2Id) && c.User2Id ==Guid.Parse(user1Id)));
        }

        public async Task<Conversation> CreateAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<List<Conversation>> GetUserConversationsAsync(string userId, int skip, int take)
        {
            return await _context.Conversations
                .Where(c => c.User1Id == Guid.Parse(userId) || c.User2Id == Guid.Parse(userId))
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Include(c => c.User1)
                .Include(c => c.User2)
                .OrderByDescending(c => c.LastActivity)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<Conversation> GetByIdAsync(string conversationId)
        {
            return await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => c.Id == Guid.Parse(conversationId));
        }

        public async Task<Conversation> UpdateAsync(Conversation conversation)
        {
            _context.Update(conversation);
             await _context.SaveChangesAsync();
            return conversation;
        }
        public async Task MarkConversationMessagesAsReadAsync(string conversationId, string userId, CancellationToken cancellationToken)
        {
            var unreadMessages = await _context.Messages
             .Where(m => m.ConversationId == Guid.Parse(conversationId)
                         && m.RecipientId == Guid.Parse(userId)
                         && m.Status != MessageStatus.Read)
             .ToListAsync(cancellationToken);

            foreach (var message in unreadMessages)
            {
                message.Status = MessageStatus.Read;
            }

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
