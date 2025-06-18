using EchoB.Domain.Entities;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Persistence.Repositories
{
    public class BlockedUserRepository : IBlockedUserRepository
    {
        private readonly EchoBDbContext _context;

        public BlockedUserRepository(EchoBDbContext context)
        {
            _context = context;
        }

        public async Task<BlockedUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.BlockedUsers
                .Include(bu => bu.User)
                .FirstOrDefaultAsync(bu => bu.Id == Guid.Parse(id), cancellationToken);
        }

        public async Task<IEnumerable<BlockedUser>> GetBlockedUsersByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.BlockedUsers
                .Where(bu => bu.UserId == Guid.Parse(userId))
                .Include(bu => bu.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BlockedUser>> GetUsersWhoBlockedUserAsync(string blockedUserId, CancellationToken cancellationToken = default)
        {
            return await _context.BlockedUsers
                .Where(bu => bu.BlockedUserId == Guid.Parse(blockedUserId))
                .Include(bu => bu.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUserBlockedAsync(string userId, string blockedUserId, CancellationToken cancellationToken = default)
        {
            return await _context.BlockedUsers
                .AnyAsync(bu => bu.UserId == Guid.Parse(userId) && bu.BlockedUserId == Guid.Parse(blockedUserId), cancellationToken);
        }

        public async Task AddAsync(BlockedUser blockedUser, CancellationToken cancellationToken = default)
        {
            await _context.BlockedUsers.AddAsync(blockedUser, cancellationToken);
        }

        public Task DeleteAsync(BlockedUser blockedUser, CancellationToken cancellationToken = default)
        {
            _context.BlockedUsers.Remove(blockedUser);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task BlockUserAsync(string blockerId, string blockedId)
        {
            

            bool alreadyBlocked = await _context.BlockedUsers
                .AnyAsync(b => b.UserId == Guid.Parse(blockerId) && b.BlockedUserId == Guid.Parse(blockedId));

            if (alreadyBlocked)
                throw new InvalidOperationException("User is already blocked.");

            var block = new BlockedUser(Guid.Parse(blockerId),Guid.Parse(blockedId));
           

            _context.BlockedUsers.Add(block);
            await _context.SaveChangesAsync();
        }
        public async Task UnblockUserAsync(string blockerId, string blockedId)
        {
            var block = await _context.BlockedUsers
                .FirstOrDefaultAsync(b => b.UserId ==Guid.Parse( blockerId) && b.BlockedUserId ==Guid.Parse( blockedId));

            if (block == null)
                throw new InvalidOperationException("No block found between the users.");

            _context.BlockedUsers.Remove(block);
            await _context.SaveChangesAsync();
        }


    }
}

