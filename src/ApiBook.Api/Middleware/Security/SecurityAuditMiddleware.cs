using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;

namespace ApiBook.Api.Middleware.Security;

/// <summary>
/// Phase 2: analyze each request, log structured security events, persist medium/high to DB (no blocking).
/// </summary>
public sealed class SecurityAuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRiskScoreEngine riskScoreEngine, ISecurityAuditTrail auditTrail)
    {
        var path = $"{context.Request.PathBase}{context.Request.Path}";
        var userAgentHeader = context.Request.Headers.UserAgent.ToString();
        if (userAgentHeader.Length > 500)
            userAgentHeader = userAgentHeader[..500];

        var snapshot = new SecurityRequestSnapshot(
            context.Request.Method,
            path,
            context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
            context.Connection.RemoteIpAddress?.ToString(),
            string.IsNullOrEmpty(userAgentHeader) ? null : userAgentHeader,
            context.User?.Identity?.IsAuthenticated ?? false,
            context.User?.Identity?.Name);

        var analysis = riskScoreEngine.Evaluate(snapshot);
        await auditTrail.AppendAsync(analysis, snapshot, context.RequestAborted);

        await next(context);
    }
}
