using System;

namespace EchoB.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        protected BaseEntity(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity? left, BaseEntity? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity? left, BaseEntity? right)
        {
            return !Equals(left, right);
        }
    }
}

