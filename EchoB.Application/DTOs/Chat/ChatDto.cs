using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Chat
{
    public class ChatDto
    {
        public string ConversationId { get; set; }
        public string OtherId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserImage { get; set; }
        public string OtherFullName { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsOnline { get; set; }
        public List<MessageDto> Messages { get; set; }

    }
}
