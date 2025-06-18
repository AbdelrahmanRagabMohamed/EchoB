using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace EchoB.Api.Filters
{
    public class ValidateModelFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = 400,
                    Instance = context.HttpContext.Request.Path
                };

                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }
    }

    public class SanitizeInputFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument != null)
                {
                    SanitizeObject(argument);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }

        private static void SanitizeObject(object obj)
        {
            if (obj == null) return;

            var properties = obj.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    // Basic XSS prevention - remove potentially dangerous characters
                    var sanitized = value
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&#x27;")
                        .Replace("/", "&#x2F;");

                    property.SetValue(obj, sanitized);
                }
            }
        }
    }
}

