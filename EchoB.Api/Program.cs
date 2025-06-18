
using EchoB.Api.Extensions;
using EchoB.Api.Hubs;
using EchoB.Api.Middleware;
using EchoB.Api.SignalR;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure;
using Serilog;
using System.Net;

namespace EchoB.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add Serilog
            builder.Services.AddSerilogLogging(builder.Configuration);
            builder.Host.UseSerilog();

            // Add services to the container
            builder.Services.AddApiServices(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddControllers();

            builder.Services.AddScoped<ISignalRService, SignalRService>();

            // Configure Kestrel to listen on all interfaces
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 5018); // HTTP
                options.Listen(IPAddress.Loopback, 7182, listenOptions =>
                {
                    listenOptions.UseHttps(); // HTTPS
                });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EchoB API V1");
                    c.RoutePrefix = "swagger"; // Set Swagger UI at the app's root
                });
            }
            // Security middleware (order matters)
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            // HTTPS redirection
            app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // CORS
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowAll");
            }
            else
            {
                app.UseCors("Production");
            }

            // Rate limiting
            app.UseRateLimiter();
            // Authentication & Authorization
            app.UseAuthentication();
            //Token Version Middleware
            app.UseMiddleware<TokenVersionMiddleware>();
            app.UseAuthorization();


            // Health checks
            app.MapHealthChecks("/health");

            // Controllers
            app.MapControllers();

            // Apply rate limiting policies to specific endpoints
            app.MapControllerRoute(
                name: "auth",
                pattern: "api/auth/{action}",
                defaults: new { controller = "Auth" })
                .RequireRateLimiting("AuthPolicy");

            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action=Index}/{id?}")
                .RequireRateLimiting("ApiPolicy");

            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<CallHub>("/callHub");
            try
            {
                Log.Information("Starting EchoB API");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "EchoB API terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
