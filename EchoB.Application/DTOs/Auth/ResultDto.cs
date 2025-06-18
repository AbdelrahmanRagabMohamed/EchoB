using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Auth
{
    public class ResultDto
    {
        public bool Succeeded { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
