using EchoB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface ICallRepository
    {
        Task<CallSession> GetByIdAsync(string id);
        Task AddAsync(CallSession call);
        Task UpdateAsync(CallSession call);
        Task<List<CallSession>> GetPendingCallsAsync();
    }
}
