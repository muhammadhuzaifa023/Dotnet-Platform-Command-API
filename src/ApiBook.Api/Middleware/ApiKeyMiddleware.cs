using ApiBook.Application.Contracts;

namespace ApiBook.Api.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> MutatingMethods =
    [
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    ];

    public async Task Invoke(HttpContext context, IApiKeyValidator apiKeyValidator)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (!MutatingMethods.Contains(context.Request.Method) || path.StartsWith("/api/auth/token", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyValue) || !apiKeyValidator.IsValid(apiKeyValue))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Missing or invalid API key." });
            return;
        }

        await next(context);
    }
}
