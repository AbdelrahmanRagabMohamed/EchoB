using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Entities
{
    public class MessageReaction:BaseEntity
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public ReactionType ReactionType { get; set; }
        public Message Message { get; set; }
        public EchoBUser User { get; set; }
        public MessageReaction(Guid messageId, Guid userId, ReactionType reactionType)
        {
            MessageId = messageId;
            UserId = userId;
            ReactionType = reactionType;
          
        }
    }

}
