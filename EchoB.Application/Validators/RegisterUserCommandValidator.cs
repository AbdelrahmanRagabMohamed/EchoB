using EchoB.Application.UseCases.Commands.Auth.Register;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EchoB.Application.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(20).WithMessage("Full name cannot be longer than 20 characters.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.")
                .Must(IsValidUserName).WithMessage("Username must be a valid phone number, email, or contain only letters, numbers, and underscores.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
        }

        private bool IsValidUserName(string username)
        {
            // Username can be:
            // 1. Email
            // 2. Phone number (digits only, 10–15 digits)
            // 3. Alphanumeric + underscore

            var isEmail = Regex.IsMatch(username, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var isPhone = Regex.IsMatch(username, @"^\d{10,15}$");
            var isSimpleUserName = Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");

            return isEmail || isPhone || isSimpleUserName;
        }
    }
}
