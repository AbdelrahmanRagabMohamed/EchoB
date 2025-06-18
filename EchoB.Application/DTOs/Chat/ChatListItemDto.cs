using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Chat
{
    public class ChatListItemDto
    {
        public string ConversationId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserImage { get; set; }
        public string LastMessageContent { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
        public string MessageStatus { get; set; }
        public int UnreadCount { get; set; }
    }
}
