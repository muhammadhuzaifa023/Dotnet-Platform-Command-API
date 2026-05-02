using System.Diagnostics;

namespace ApiBook.Api.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            await _next(context);

            sw.Stop();

            var time = sw.ElapsedMilliseconds;
            var status = context.Response.StatusCode;
            var path = context.Request.Path;

            _logger.LogInformation("API {Path} responded {Status} in {Time} ms", path, status, time);

            if (time > 500)
            {
                _logger.LogWarning("⚠️ Slow API {Path} took {Time} ms", path, time);
            }
        }
    }
}
