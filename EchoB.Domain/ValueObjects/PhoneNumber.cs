using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EchoB.Domain.ValueObjects
{

    public sealed class PhoneNumber : IEquatable<PhoneNumber>
    {
        private static readonly Regex _phoneRegex = new(@"^\+?[1-9]\d{7,14}$");

        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty.");

            var normalized = Normalize(phoneNumber);

            if (!_phoneRegex.IsMatch(normalized))
                throw new ArgumentException("Invalid phone number format.");

            return new PhoneNumber(normalized);
        }

        private static string Normalize(string number)
        {
            return number.Trim().Replace(" ", "").Replace("-", "");
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

        public bool Equals(PhoneNumber? other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    }

}
