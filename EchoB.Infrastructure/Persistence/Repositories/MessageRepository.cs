using EchoB.Application.DTOs;
using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly EchoBDbContext _context;

        public MessageRepository(EchoBDbContext context)
        {
            _context = context;
        }

        public async Task<Message> CreateAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetByConversationAsync(string conversationId,string userId, int skip, int take)
        {

            return await _context.Messages
                .Where(m => m.ConversationId == Guid.Parse(conversationId) &&
                    !((m.SenderId == Guid.Parse(userId) && m.IsDeletedForSender) ||
                        (m.RecipientId == Guid.Parse(userId) && m.IsDeletedForRecipient)))
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
                }

        public async Task UpdateStatusAsync(string messageId, MessageStatus status)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.UpdateStatus(status);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Message> GetByIdAsync(string messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public async Task<Message> UpdateAsync(Message message)
        {
            _context.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }
        public async Task<MessageReaction> AddMessageReactionAsync(string messageId, string userId, ReactionType reactionType)
        {
            var message = await _context.Messages
                .Include(m => m.Reactions)
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse(messageId));

            if (message == null)
                throw new UserNotFoundException("Message not found.");

            // Check for existing reaction by this user on the same message
            var existingReaction = message.Reactions
                .FirstOrDefault(r => r.UserId == Guid.Parse(userId));

            if (existingReaction != null)
            {
                // If reaction type is same, return it (no need to change)
                if (existingReaction.ReactionType == reactionType)
                    return existingReaction;

                // Else update the existing reaction type
                existingReaction.ReactionType = reactionType;

                _context.MessagesReactions.Update(existingReaction);
                await _context.SaveChangesAsync();

                return existingReaction;
            }

            // If no previous reaction exists, create a new one
            var newReaction = new MessageReaction(Guid.Parse(messageId),Guid.Parse(userId),reactionType);
            

            _context.MessagesReactions.Add(newReaction);
            await _context.SaveChangesAsync();

            return newReaction;
        }

        public async Task DeleteMessageAsync(string messageId, string userId,bool deleteForAll)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == Guid.Parse(messageId));

            if (message == null)
                throw new UserNotFoundException("Message not found.");

            if (message.SenderId == Guid.Parse(userId))
            {
                if(deleteForAll)
                    message.IsDeletedForRecipient = true;
                message.IsDeletedForSender = true;
                message.DeletedAt = DateTime.UtcNow;
            }
            else if (message.RecipientId == Guid.Parse(userId))
            {
                message.IsDeletedForRecipient = true;
            }
            else
            {
                throw new UnauthorizedAccessException("User is not part of this conversation.");
            }

            await _context.SaveChangesAsync();
        }
        public async Task<Message> EditMessageAsync(string messageId, string userId, string newContent)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse(messageId));

            if (message == null)
                throw new UserNotFoundException("Message not found.");

            if (message.SenderId != Guid.Parse(userId))
                throw new UnauthorizedAccessException("Only the sender can edit the message.");

            message.Content = newContent;

            _context.Messages.Update(message);
            await _context.SaveChangesAsync();

            return message;
        }


    }
}
