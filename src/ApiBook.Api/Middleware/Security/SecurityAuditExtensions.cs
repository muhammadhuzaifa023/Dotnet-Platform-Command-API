namespace ApiBook.Api.Middleware.Security;

public static class SecurityAuditExtensions
{
    public static IApplicationBuilder UseSecurityAudit(this IApplicationBuilder app) =>
        app.UseMiddleware<SecurityAuditMiddleware>();
}
