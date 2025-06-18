using System;

namespace EchoB.Domain.Entities
{
    public class BlockedUser : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid BlockedUserId { get; private set; }

        // Navigation properties
        public EchoBUser User { get; private set; } = null!;

        // Private constructor for EF Core
        private BlockedUser() 
        {
            User = null!;
        }

        public BlockedUser(Guid userId, Guid blockedUserId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (blockedUserId == Guid.Empty)
                throw new ArgumentException("Blocked user ID cannot be empty.", nameof(blockedUserId));

            if (userId == blockedUserId)
                throw new ArgumentException("User cannot block themselves.");

            UserId = userId;
            BlockedUserId = blockedUserId;
        }
    }
}

