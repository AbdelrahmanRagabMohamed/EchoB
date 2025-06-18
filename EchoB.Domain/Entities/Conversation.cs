using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Entities
{
    public class Conversation :BaseEntity
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public EchoBUser User1 { get; set; }
        public EchoBUser User2 { get; set; }
        public List<Message> Messages { get; set; } = new();
        public DateTime LastActivity { get; set; }
    }
}
