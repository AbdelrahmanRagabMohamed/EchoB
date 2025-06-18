namespace EchoB.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var requestId = Guid.NewGuid().ToString();
            
            // Add request ID to response headers for tracking
            context.Response.Headers["X-Request-ID"] = requestId;

            // Log request details
            _logger.LogInformation("Request {RequestId} started: {Method} {Path} from {IpAddress} at {StartTime}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                GetClientIpAddress(context),
                startTime);

            try
            {
                await _next(context);
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                // Log response details
                _logger.LogInformation("Request {RequestId} completed: {StatusCode} in {Duration}ms at {EndTime}",
                    requestId,
                    context.Response.StatusCode,
                    duration.TotalMilliseconds,
                    endTime);

                // Log security-relevant events
                if (context.Response.StatusCode == 401)
                {
                    _logger.LogWarning("Unauthorized access attempt from {IpAddress} to {Path}",
                        GetClientIpAddress(context),
                        context.Request.Path);
                }
                else if (context.Response.StatusCode == 403)
                {
                    _logger.LogWarning("Forbidden access attempt from {IpAddress} to {Path}",
                        GetClientIpAddress(context),
                        context.Request.Path);
                }
                else if (context.Response.StatusCode >= 500)
                {
                    _logger.LogError("Server error {StatusCode} for request {RequestId} from {IpAddress}",
                        context.Response.StatusCode,
                        requestId,
                        GetClientIpAddress(context));
                }
            }
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (in case of proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}

