using System.Net;
using System.Text.Json;

namespace WorldMonitor.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _log;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpRequestException ex)
        {
            _log.LogError(ex, "External HTTP request failed");
            await WriteErrorAsync(context, HttpStatusCode.BadGateway,
                "External service unavailable", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _log.LogWarning(ex, "Invalid operation");
            await WriteErrorAsync(context, HttpStatusCode.BadRequest,
                "Invalid operation", ex.Message);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred", ex.Message);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode status, string error, string detail)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        var payload = JsonSerializer.Serialize(new { error, detail });
        await context.Response.WriteAsync(payload);
    }
}
