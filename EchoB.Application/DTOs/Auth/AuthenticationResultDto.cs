using EchoB.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Auth
{
    public class AuthenticationResultDto : ResultDto
    {
        public TokenValues Token { get; set; } = null!;
    }
}
