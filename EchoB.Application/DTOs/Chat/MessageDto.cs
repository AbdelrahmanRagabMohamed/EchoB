using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Chat
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string? ReplyToMessageId { get; set; }
        public List<MessageReactionDto> Reactions { get; set; } = new();
    }
}
