using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Persistence.Repositories
{
    public class CallRepository : ICallRepository
    {
        private readonly EchoBDbContext _context;
        public CallRepository(EchoBDbContext context)
        {
            _context = context;
        }
        public async Task<CallSession> GetByIdAsync(string id)
        {
            return await _context.Calls.FindAsync(id);
        }
        public async Task AddAsync(CallSession call)
        {
            await _context.Calls.AddAsync(call);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(CallSession call)
        {
            _context.Calls.Update(call);
            await _context.SaveChangesAsync();
        }
        public async Task<List<CallSession>> GetPendingCallsAsync()
        {
            return await _context.Calls.Where(c => c.Status == CallStatus.Pending).ToListAsync();
        }
    }
}
