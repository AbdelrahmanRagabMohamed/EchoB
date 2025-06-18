namespace EchoB.Infrastructure.Configuration
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenExpirationDays { get; set; } = 7;
    }

    public class EmailSettings
    {
        public string SMTPHost { get; set; } = string.Empty;
        public int SMTPPort { get; set; } = 587;
        public string SMTPUser { get; set; } = string.Empty;
        public string SMTPPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string OTPHtmlBodyTemplate { get; set; } = "<h3>Your verification code is: {code}</h3>";
    }

    public class AISettings
    {
        public string WebSocketUrl { get; set; }
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName { get; set; } = "EcoB";
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public bool EnableSensitiveDataLogging { get; set; } = false;
        public bool EnableDetailedErrors { get; set; } = false;
    }
}

