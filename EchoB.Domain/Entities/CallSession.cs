using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Entities
{
    public class CallSession
    {
        public Guid Id { get; set; }
        public Guid CallerId { get; set; }
        public Guid ReceiverId { get; set; }
        public CallStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
