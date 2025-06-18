using EchoB.Domain.Entities;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Application.Interfaces
{
    public class TokenValues
    {
        public string? Token { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
    public interface ITokenService
    {
        Task<TokenValues> GenerateAccessTokenAsync(EchoBUser user, CancellationToken cancellationToken = default);
       
    }
}

