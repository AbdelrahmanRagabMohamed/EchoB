using EchoB.Application.Behaviors;
using EchoB.Application.Common;
using EchoB.Application.Validators;
using EchoB.Infrastructure;
using EchoB.Api.Middleware;
using EchoB.Api.Filters;
using FluentValidation;
using MediatR;
using Serilog;
using System.Reflection;
using EchoB.Application.UseCases.Commands.Auth.Register;

namespace EchoB.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add global filters
            services.AddScoped<IpBlockingFilter>();
            services.AddScoped<ValidateModelFilter>();
            services.AddScoped<SanitizeInputFilter>();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
                options.Filters.Add<SanitizeInputFilter>();
            });

            // Add API Explorer for Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "EchoB API",
                    Version = "v1",
                    Description = "A secure and scalable social media backend API built with Clean Architecture",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "EchoB Team",
                        Email = "echobteam@gmail.com"
                    }
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Add JWT authentication to Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });

            // Add FluentValidation
            services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

            // Add AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });

                options.AddPolicy("Production", builder =>
                {
                    builder
                        .WithOrigins("https://yourdomain.com") // Replace with actual domain
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // Add rate limiting
            RateLimitingMiddleware.ConfigureRateLimiting(services);

            // Add health checks
            services.AddHealthChecks()
                .AddDbContextCheck<Infrastructure.Persistence.Context.EchoBDbContext>();

            return services;
        }

        public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/ecob-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSerilog();

            return services;
        }
    }
}

