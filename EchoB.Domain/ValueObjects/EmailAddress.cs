using System;
using System.Text.RegularExpressions;

namespace EchoB.Domain.ValueObjects
{
    public class EmailAddress : IEquatable<EmailAddress>
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email address cannot be null or empty.", nameof(value));

            var normalizedEmail = value.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(normalizedEmail))
                throw new ArgumentException("Invalid email address format.", nameof(value));

            Value = normalizedEmail;
        }

        public bool Equals(EmailAddress? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as EmailAddress);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(EmailAddress emailAddress)
        {
            return emailAddress.Value;
        }

        public static implicit operator EmailAddress(string value)
        {
            return new EmailAddress(value);
        }
    }
}

