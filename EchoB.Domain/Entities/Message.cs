using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Entities
{

    public class Message :BaseEntity
    {
        public Guid ConversationId { get;  set; }
        public Guid SenderId { get;  set; }
        public Guid RecipientId {  get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public MessageStatus Status { get; set; }
        public Guid? ReplyToMessageId { get; set; }
        public bool IsDeletedForSender { get; set; } = false;
        public bool IsDeletedForRecipient { get; set; } = false;
        public DateTime DeletedAt { get; set; }
        public List<MessageReaction> Reactions { get; set; } = new();
        public MessageStatus MessageStatuses { get; set; } = MessageStatus.Sent;

        public void UpdateStatus(MessageStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

       

        public void AddReaction(Guid userId, ReactionType reactionType)
        {
            var existingReaction = Reactions.FirstOrDefault(r => r.UserId == userId && r.ReactionType == reactionType);
            if (existingReaction == null)
            {
                Reactions.Add(new MessageReaction(Id, userId, reactionType));
            }
        }
    }
}
