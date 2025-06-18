using System;

namespace EchoB.Domain.ValueObjects
{
    public class Bio : IEquatable<Bio>
    {
        public string? Value { get; }

        public Bio(string? value)
        {
            if (value != null)
            {
                if (value.Length > 200)
                    throw new ArgumentException("Bio cannot be longer than 200 characters.", nameof(value));

                Value = value.Trim();
            }
            else
            {
                Value = null;
            }
        }

        public bool Equals(Bio? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Bio);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }

        public static implicit operator string?(Bio bio)
        {
            return bio.Value;
        }

        public static implicit operator Bio(string? value)
        {
            return new Bio(value);
        }
    }
}

