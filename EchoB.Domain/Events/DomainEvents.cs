using System;

namespace EchoB.Domain.Events
{
    public abstract class DomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }

    public class UserRegisteredEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public UserRegisteredEvent(Guid userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }

    public class UserEmailConfirmedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }

        public UserEmailConfirmedEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }

    public class UserPasswordChangedEvent : DomainEvent
    {
        public Guid UserId { get; }

        public UserPasswordChangedEvent(Guid userId)
        {
            UserId = userId;
        }
    }

    public class UserBlockedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public Guid BlockedUserId { get; }

        public UserBlockedEvent(Guid userId, Guid blockedUserId)
        {
            UserId = userId;
            BlockedUserId = blockedUserId;
        }
    }

    public class UserUnblockedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public Guid UnblockedUserId { get; }

        public UserUnblockedEvent(Guid userId, Guid unblockedUserId)
        {
            UserId = userId;
            UnblockedUserId = unblockedUserId;
        }
    }
}

