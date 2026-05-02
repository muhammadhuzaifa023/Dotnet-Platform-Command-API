namespace ApiBook.Api.Middleware.Security
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ✅ MIME type sniffing band karo
            // Browser file type guess na kare
            context.Response.Headers.Append(
                "X-Content-Type-Options", "nosniff");

            // ✅ Clickjacking band karo
            // Tera API kisi aur website ke iframe mein load na ho
            context.Response.Headers.Append(
                "X-Frame-Options", "DENY");

            // ✅ XSS Filter (purane browsers ke liye)
            context.Response.Headers.Append(
                "X-XSS-Protection", "1; mode=block");

            // ✅ Referrer info control karo
            // Doosri site ko tera URL nazar na aaye
            context.Response.Headers.Append(
                "Referrer-Policy", "strict-origin-when-cross-origin");

            // ✅ Sirf HTTPS pe chalo (production mein)
            // 1 saal tak browser HTTP use nahi karega
            context.Response.Headers.Append(
                "Strict-Transport-Security",
                "max-age=31536000; includeSubDomains");

            // ✅ Browser features band karo jo API ko chahiye nahi
            context.Response.Headers.Append(
                "Permissions-Policy",
                "geolocation=(), microphone=(), camera=(), " +
                "payment=(), usb=(), bluetooth=()");

            // ✅ Content Security Policy
            // Sirf apna content allow karo
            context.Response.Headers.Append(
                "Content-Security-Policy", "default-src 'self'");

            // ✅ Server info hide karo
            // Attacker ko .NET version pata na chale
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-AspNetMvc-Version");

            await _next(context);
        }
    }
}
