using System;

namespace EchoB.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException(Guid userId) : base($"User with ID '{userId}' was not found.")
        {
        }

        public UserNotFoundException(string identifier) : base($"User with identifier '{identifier}' was not found.")
        {
        }
    }
    public class UserNotVerifiedException : DomainException
    {
        public UserNotVerifiedException(Guid userId) : base($"User with ID '{userId}' was not Verified.")
        {
        }

        public UserNotVerifiedException(string identifier) : base($"User with identifier '{identifier}' was not verified.")
        {
        }
        public UserNotVerifiedException() : base($"User account is Not Verified.")
        {

        }
    }

    public class UserAlreadyExistsException : DomainException
    {
        public UserAlreadyExistsException(string identifier) : base($"User with identifier '{identifier}' already exists.")
        {
        }

    }
    public class UserAlreadyVerifiedException : DomainException
    {
        public UserAlreadyVerifiedException(string identifier) : base($"User with identifier '{identifier}' already Verified.")
        {
        }

    }

    public class InvalidUserOperationException : DomainException
    {
        public InvalidUserOperationException(string message) : base(message)
        {
        }
    }

    public class UserAccountLockedException : DomainException
    {
        public DateTimeOffset LockoutEnd { get; }

        public UserAccountLockedException(DateTimeOffset lockoutEnd) : base($"User account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss} UTC.")
        {
            LockoutEnd = lockoutEnd;
        }
        public UserAccountLockedException() : base($"User account is locked.")
        {
            
        }
    }
   
}

