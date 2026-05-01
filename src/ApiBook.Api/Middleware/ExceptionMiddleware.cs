using System.Net;
using System.Text.Json;

namespace ApiBook.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled Exception Occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "Something went wrong",
                detail = ex.Message // dev me useful
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}