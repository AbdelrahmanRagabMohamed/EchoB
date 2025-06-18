using EchoB.Application.UseCases.Commands.Account;
using EchoB.Application.UseCases.Commands.Auth.Login;
using FluentValidation;

namespace EchoB.Application.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Email or username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }

    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
           

            When(x => !string.IsNullOrWhiteSpace(x.FullName), () =>
            {
                RuleFor(x => x.FullName)
                    .MaximumLength(20).WithMessage("Full name cannot be longer than 20 characters.");
            });

            When(x => x.Bio != null, () =>
            {
                RuleFor(x => x.Bio)
                    .MaximumLength(200).WithMessage("Bio cannot be longer than 200 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.ProfilePictureUrl), () =>
            {
                RuleFor(x => x.ProfilePictureUrl)
                    .Must(BeAValidUrl).WithMessage("Profile picture URL must be a valid HTTP or HTTPS URL.")
                    .MaximumLength(200).WithMessage("Profile picture URL cannot be longer than 200 characters.");
            });

            When(x => x.DateOfBirth.HasValue, () =>
            {
                RuleFor(x => x.DateOfBirth)
                    .LessThan(DateTime.UtcNow.AddYears(-13)).WithMessage("User must be at least 13 years old.");
            });
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
           

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("New password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password.");
        }
    }
}

