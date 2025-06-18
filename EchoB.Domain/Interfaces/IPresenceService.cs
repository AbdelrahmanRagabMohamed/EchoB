using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface IPresenceService
    {
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string userId, string connectionId);
        Task<bool> IsUserOnlineAsync(string userId);
        Task PublishMessageAsync<T>(string userId, string eventName, T payload);
        Task<List<string>> GetConnectionsAsync(string userId);
    }
}
