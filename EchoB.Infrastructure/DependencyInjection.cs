using DeltaCore.CacheHelper;
using DeltaCore.EmailService;
using DeltaCore.SMSService;
using EchoB.Application.Interfaces;
using EchoB.Domain.AI;
using EchoB.Domain.Entities;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.AI;
using EchoB.Infrastructure.BackgroundServices;
using EchoB.Infrastructure.Configuration;
using EchoB.Infrastructure.Persistence.Context;
using EchoB.Infrastructure.Persistence.Repositories;
using EchoB.Infrastructure.Security;
using EchoB.Infrastructure.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace EchoB.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<EchoBDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection") 
                    ?? throw new InvalidOperationException("Database connection string is not configured");
                
                options.UseSqlServer(connectionString, b => 
                {
                    b.MigrationsAssembly(typeof(EchoBDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });

                var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
                if (databaseSettings != null)
                {
                    if (databaseSettings.EnableSensitiveDataLogging)
                        options.EnableSensitiveDataLogging();
                    
                    if (databaseSettings.EnableDetailedErrors)
                        options.EnableDetailedErrors();
                }
            });
            services.AddIdentity<EchoBUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<EchoBDbContext>()
                .AddDefaultTokenProviders();
            // Repositories
            services.AddScoped<IBlockedUserRepository, BlockedUserRepository>();
            services.AddScoped<ICallRepository, CallRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // SignalR
            services.AddSignalR();
            // Domain Services

            // Application Services
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ISMSService, TwilioService>();
            services.AddScoped<IEmailService, SMTPService>();
            services.AddScoped<RedisCacheService>();
            services.AddSingleton<MemoryCacheService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            services.Configure<AISettings>(configuration.GetSection("AI"));
            services.AddScoped<IPresenceService, PresenceService>();
            services.AddHostedService<CallTimeoutService>();
            services.AddMemoryCache();


            services.AddScoped<ISignLanguageProcessor, SignLanguageProcessor>();
            // HTTP Client for external services

            // Redis Cache
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = configuration.GetSection("RedisSettings:InstanceName").Value ?? "EcoB";
                });
            }
            else
            {
                // Fallback to in-memory cache if Redis is not configured
                services.AddMemoryCache();
            }
            // Add SignInManager and UserClaimsPrincipalFactory manually
            services.AddScoped<SignInManager<EchoBUser>>();
            services.AddScoped<IUserClaimsPrincipalFactory<EchoBUser>, UserClaimsPrincipalFactory<EchoBUser>>();

            // JWT Authentication
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() 
                ?? throw new InvalidOperationException("JWT settings are not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true; // Set to false only in development
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Configuration Settings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<SMTPSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

            return services;
        }
    }
}

