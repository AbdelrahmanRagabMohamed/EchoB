using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;

namespace EchoB.Api.Filters
{
    public class IpBlockingFilter : IAsyncActionFilter
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<IpBlockingFilter> _logger;
        private readonly int _maxAttempts;
        private readonly TimeSpan _blockDuration;

        public IpBlockingFilter(IDistributedCache cache, ILogger<IpBlockingFilter> logger)
        {
            _cache = cache;
            _logger = logger;
            _maxAttempts = 10; // Max failed attempts before blocking
            _blockDuration = TimeSpan.FromMinutes(15); // Block duration
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ipAddress = GetClientIpAddress(context.HttpContext);
            var cacheKey = $"blocked_ip:{ipAddress}";

            // Check if IP is currently blocked
            var blockedUntil = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(blockedUntil) && DateTime.TryParse(blockedUntil, out var blockTime))
            {
                if (DateTime.UtcNow < blockTime)
                {
                    _logger.LogWarning("Blocked IP {IpAddress} attempted access", ipAddress);
                    context.Result = new ObjectResult(new { error = "IP address is temporarily blocked" })
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                    return;
                }
                else
                {
                    // Block has expired, remove from cache
                    await _cache.RemoveAsync(cacheKey);
                }
            }

            var executedContext = await next();

            // Check if this was a failed authentication attempt
            if (executedContext.Result is ObjectResult objectResult && 
                objectResult.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await HandleFailedAttempt(ipAddress);
            }
        }

        private async Task HandleFailedAttempt(string ipAddress)
        {
            var attemptKey = $"failed_attempts:{ipAddress}";
            var attemptsStr = await _cache.GetStringAsync(attemptKey);
            
            int attempts = 0;
            if (!string.IsNullOrEmpty(attemptsStr))
            {
                int.TryParse(attemptsStr, out attempts);
            }

            attempts++;

            if (attempts >= _maxAttempts)
            {
                // Block the IP
                var blockUntil = DateTime.UtcNow.Add(_blockDuration);
                var blockKey = $"blocked_ip:{ipAddress}";
                
                await _cache.SetStringAsync(blockKey, blockUntil.ToString(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = blockUntil
                });

                // Remove the attempts counter
                await _cache.RemoveAsync(attemptKey);

                _logger.LogWarning("IP {IpAddress} has been blocked for {Duration} minutes due to {Attempts} failed attempts", 
                    ipAddress, _blockDuration.TotalMinutes, attempts);
            }
            else
            {
                // Update attempts counter
                await _cache.SetStringAsync(attemptKey, attempts.ToString(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Reset counter after 5 minutes
                });

                _logger.LogInformation("Failed attempt {Attempts}/{MaxAttempts} for IP {IpAddress}", 
                    attempts, _maxAttempts, ipAddress);
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

