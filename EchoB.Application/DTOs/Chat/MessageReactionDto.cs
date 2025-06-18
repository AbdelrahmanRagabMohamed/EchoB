using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Chat
{
    public class MessageReactionDto
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public ReactionType ReactionType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
