using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Call
{
    public class AnswerRequestDto
    {
        public string CallId { get; set; }
        public bool Accept { get; set; }
    }
}
