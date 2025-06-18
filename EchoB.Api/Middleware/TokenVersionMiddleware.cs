using EchoB.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EchoB.Api.Middleware
{
    public class TokenVersionMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenVersionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userManager = context.RequestServices.GetRequiredService<UserManager<EchoBUser>>();

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tokenVersionClaim = context.User.FindFirst("tokenVersion")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenVersionClaim))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid token.");
                    return;
                }

                var user = await userManager.FindByIdAsync(userId);
                if (user == null || user.TokenVersion.ToString() != tokenVersionClaim)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token has been revoked.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
