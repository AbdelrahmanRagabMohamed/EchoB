using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
