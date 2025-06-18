using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.Auth
{
    public class RegisteredContactDTO
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
