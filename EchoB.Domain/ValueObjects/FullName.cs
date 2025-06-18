using System;

namespace EchoB.Domain.ValueObjects
{
    public class FullName : IEquatable<FullName>
    {
        public string Value { get; }

        public FullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Full name cannot be null or empty.", nameof(value));

            if (value.Length > 20)
                throw new ArgumentException("Full name cannot be longer than 20 characters.", nameof(value));

            Value = value.Trim();
        }

        public bool Equals(FullName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FullName);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(FullName fullName)
        {
            return fullName.Value;
        }

        public static implicit operator FullName(string value)
        {
            return new FullName(value);
        }
    }
}

