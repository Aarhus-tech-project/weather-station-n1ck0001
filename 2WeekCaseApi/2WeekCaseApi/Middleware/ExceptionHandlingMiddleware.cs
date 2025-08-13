using System.Diagnostics;
using System.Text.Json;

namespace _2WeekCaseApi.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // Proceed to the next middleware or endpoint
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            if (Debugger.IsAttached) throw;

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = JsonSerializer.Serialize(new
        {
            message = "An unexpected error occurred.",
            details = exception.Message // For production, return user-friendly message 
        });

        return context.Response.WriteAsync(result);
    }
}