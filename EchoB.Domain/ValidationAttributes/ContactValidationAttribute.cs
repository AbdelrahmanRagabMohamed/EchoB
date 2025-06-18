using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace EchoB.Domain.ValidationAttributes
{
    public class ContactValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("The contact field is required.");
            }

            string input = value.ToString()!;

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            string phonePattern = @"^\+?[0-9]{10,15}$";

            if (Regex.IsMatch(input, emailPattern) || Regex.IsMatch(input, phonePattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid contact format. Enter a valid email or phone number.");
        }
    }
}
