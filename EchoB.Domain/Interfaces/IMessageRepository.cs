using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message);
        Task DeleteMessageAsync(string messageId, string userId,bool deleteForAll);
        Task<Message> EditMessageAsync(string messageId, string userId, string newContent);
        Task<Message> UpdateAsync(Message message);
        Task<List<Message>> GetByConversationAsync(string conversationId,string userId, int skip, int take);
        Task UpdateStatusAsync(string messageId, MessageStatus status);
        Task<Message> GetByIdAsync(string messageId);
        Task<MessageReaction> AddMessageReactionAsync(string messageId, string userId, ReactionType reactionType);
    }
}
