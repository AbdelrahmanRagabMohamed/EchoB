using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Call
{
    public class CallResponseDto
    {
        public Guid CallId { get; set; }
        public CallStatus Status { get; set; }
        public DateTime StartTime { get; set; }
    }
}
