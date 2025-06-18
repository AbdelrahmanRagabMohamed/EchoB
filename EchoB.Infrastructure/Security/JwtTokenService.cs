using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Security
{
   
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpirationMinutes;
        private readonly int _tokenExpirationDays;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
            _tokenExpirationDays = int.Parse(_configuration["Jwt:TokenExpirationDays"] ?? "7");
        }

        public Task<TokenValues> GenerateAccessTokenAsync(EchoBUser user, CancellationToken cancellationToken = default)
        {
            var Claims = new List<Claim>();
            Claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            Claims.Add(new Claim("tokenVersion", user.TokenVersion.ToString()));
            Claims.Add(new Claim("FullName", user.FullName));
            Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            TokenValues tokenValues = new TokenValues();
            tokenValues = GenerateToken(Claims, TimeSpan.FromDays(_tokenExpirationDays));


            return Task.FromResult(tokenValues);

        }

        

        private TokenValues GenerateToken(IEnumerable<Claim> claims, TimeSpan expiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: credentials
            );
            TokenValues tokenValues = new TokenValues();
            tokenValues.Token = new JwtSecurityTokenHandler().WriteToken(token);
            tokenValues.ExpirationTime = token.ValidTo;
            return tokenValues;
        }
    }
}

