using EchoB.Domain.Exceptions;
using System;

namespace EchoB.Application.Common
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {
        }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ValidationException : ApplicationException
    {
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(FluentValidation.Results.ValidationResult validationResult)
            : base(BuildErrorMessage(validationResult))
        {
        }

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : base(BuildErrorMessage(failures))
        {
        }

        private static string BuildErrorMessage(FluentValidation.Results.ValidationResult validationResult)
        {
            return BuildErrorMessage(validationResult.Errors);
        }

        private static string BuildErrorMessage(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        {
            var errors = failures.Select(f => f.ErrorMessage);
            return string.Join(Environment.NewLine, errors);
        }
    }

    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }

    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
    public class UserBlockedException : ApplicationException
    {
        public UserBlockedException() : base("User Blocked") { }
    }
}

