using System.Net;
using System.Text.Json;

namespace Mezan.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "حدث خطأ غير متوقع في الخادم، يرجى المحاولة لاحقاً.";

        switch (exception)
        {
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case InvalidOperationException:
            case ArgumentException:
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
        }

        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = (int)code;

        var response = new
        {
            statusCode = (int)code,
            message
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}
