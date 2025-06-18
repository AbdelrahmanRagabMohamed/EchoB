using EchoB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface IBlockedUserRepository
    {
        Task<BlockedUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlockedUser>> GetBlockedUsersByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlockedUser>> GetUsersWhoBlockedUserAsync(string blockedUserId, CancellationToken cancellationToken = default);
        Task<bool> IsUserBlockedAsync(string userId, string blockedUserId, CancellationToken cancellationToken = default);
        Task AddAsync(BlockedUser blockedUser, CancellationToken cancellationToken = default);
        Task DeleteAsync(BlockedUser blockedUser, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BlockUserAsync(string blockerId, string blockedId);
        Task UnblockUserAsync(string blockerId, string blockedId);
    }
}

