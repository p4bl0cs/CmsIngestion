using System.Net.Http.Headers;
using System.Text;

namespace CmsIngestion.Api.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BasicAuthMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public BasicAuthMiddleware(RequestDelegate next, ILogger<BasicAuthMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Intercept requests specifically targeting the webhook endpoint
            if (!context.Request.Path.StartsWithSegments("/cms/events", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogWarning("Webhook request failed: Missing Authorization header.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Missing Authorization Header\"}");
                return;
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                if (!string.Equals(authHeader.Scheme, "Basic", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"Invalid Authentication Scheme\"}");
                    return;
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"Invalid Authorization Header Format\"}");
                    return;
                }

                var username = credentials[0];
                var password = credentials[1];

                var configuredUsername = _configuration["BasicAuth:Username"];
                var configuredPassword = _configuration["BasicAuth:Password"];

                Console.WriteLine($"[DEBUG] Incoming -> User: '{username}' | Pass: '{password}'");
                Console.WriteLine($"[DEBUG] Config   -> User: '{configuredUsername}' | Pass: '{configuredPassword}'");

                // Validate format rules AND match against configured values
                if (username.Length < 10 || username.Length > 20 ||
                    !Guid.TryParse(password, out _) ||
                    username != configuredUsername ||
                    password != configuredPassword)
                {
                    _logger.LogWarning("Webhook request failed: Invalid credentials provided.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"Invalid Username or Password\"}");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Basic Authentication header.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Invalid Authorization Header\"}");
            }

            // Authentication passed -> Hand off to the next middleware/controller in the pipeline
            await _next(context);
        }
    }

    // Clean Extension Method for Program.cs registration
    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}
