using System;

namespace EchoB.Domain.ValueObjects
{
    public class ProfilePictureUrl : IEquatable<ProfilePictureUrl>
    {
        public string? Value { get; }

        public ProfilePictureUrl(string? value)
        {
            if (value != null)
            {
                if (value.Length > 200)
                    throw new ArgumentException("Profile picture URL cannot be longer than 200 characters.", nameof(value));

                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || 
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    throw new ArgumentException("Profile picture URL must be a valid HTTP or HTTPS URL.", nameof(value));

                Value = value.Trim();
            }
            else
            {
                Value = null;
            }
        }

        public bool Equals(ProfilePictureUrl? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProfilePictureUrl);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }

        public static implicit operator string?(ProfilePictureUrl profilePictureUrl)
        {
            return profilePictureUrl.Value;
        }

        public static implicit operator ProfilePictureUrl(string? value)
        {
            return new ProfilePictureUrl(value);
        }
    }
}

