using EchoB.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly IConnectionMultiplexer _redis;

        public PresenceService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task AddConnectionAsync(string userId, string connectionId)
        {
            var db = _redis.GetDatabase();
            await db.SetAddAsync($"user:{userId}:connections", connectionId);
        }

        public async Task RemoveConnectionAsync(string userId, string connectionId)
        {
            var db = _redis.GetDatabase();
            await db.SetRemoveAsync($"user:{userId}:connections", connectionId);
        }

        public async Task<bool> IsUserOnlineAsync(string userId)
        {
            var db = _redis.GetDatabase();
            var connections = await db.SetMembersAsync($"user:{userId}:connections");
            return connections.Length > 0;
        }

        public async Task PublishMessageAsync<T>(string userId, string eventName, T payload)
        {
            var db = _redis.GetDatabase();
            var channel = $"messages:{userId}";
            await db.PublishAsync(channel, JsonSerializer.Serialize(new { EventName = eventName, Payload = payload }));
        }
    
    public async Task<List<string>> GetConnectionsAsync(string userId)
        {
            var db = _redis.GetDatabase();
            var connections = await db.SetMembersAsync($"user:{userId}:connections");

            return connections
                .Select(c => c.ToString())
                .ToList();
        }
    }
}