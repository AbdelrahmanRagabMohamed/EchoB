using EchoB.Application.Common;
using EchoB.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidationException = FluentValidation.ValidationException;

namespace EchoB.Api.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var problemDetails = exception switch
            {
                FluentValidationException validationEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation Error",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = validationEx.Message,
                    Instance = context.Request.Path
                },
                
                UserNotFoundException userNotFoundEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "User Not Found",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = userNotFoundEx.Message,
                    Instance = context.Request.Path
                },
                
                UserAlreadyExistsException userExistsEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Title = "User Already Exists",
                    Status = (int)HttpStatusCode.Conflict,
                    Detail = userExistsEx.Message,
                    Instance = context.Request.Path
                },
                
                InvalidUserOperationException invalidOpEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid Operation",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = invalidOpEx.Message,
                    Instance = context.Request.Path
                },
                
                UserAccountLockedException lockedEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Account Locked",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = lockedEx.Message,
                    Instance = context.Request.Path
                },
                
                UnauthorizedAccessException => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = "Authentication is required to access this resource",
                    Instance = context.Request.Path
                },
                
                ForbiddenException forbiddenEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Forbidden",
                    Status = (int)HttpStatusCode.Forbidden,
                    Detail = forbiddenEx.Message,
                    Instance = context.Request.Path
                },
                
                DomainException domainEx => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Domain Error",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = domainEx.Message,
                    Instance = context.Request.Path
                },
                
                _ => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = "An unexpected error occurred",
                    Instance = context.Request.Path
                }
            };

            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(problemDetails, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            }));
        }
    }
}

