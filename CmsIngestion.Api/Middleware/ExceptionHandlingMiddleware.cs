using System.Net;
using System.Text.Json;

namespace CmsIngestion.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var traceId = context.TraceIdentifier;

                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

                await HandleExceptionAsync(context, ex, traceId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
        {
            context.Response.ContentType = "application/problem+json";

            var statusCode = exception switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            // ProblemDetails standard dictionary payload
            var problemDetails = new Dictionary<string, object?>
            {
                ["status"] = (int)statusCode,
                ["title"] = GetTitleForStatusCode(statusCode),
                ["detail"] = exception.Message,
                ["instance"] = context.Request.Path,
                ["traceId"] = traceId
            };

            // Include stack trace only if running in Development
            if (_env.IsDevelopment())
            {
                problemDetails["exception"] = exception.GetType().FullName;
                problemDetails["stackTrace"] = exception.StackTrace;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problemDetails, options);

            await context.Response.WriteAsync(json);
        }

        private static string GetTitleForStatusCode(HttpStatusCode statusCode) => statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.Unauthorized => "Unauthorized",
            _ => "An error occurred while processing your request."
        };
    }
}
