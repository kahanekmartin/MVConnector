using MatrixVision.Connector.Core;
using System.Net;

namespace MatrixVision.Connector.API.App;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is ArgumentException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is InvalidOperationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        var result = Response.Error(exception.Message, exception.GetType().Name);

        return context.Response.WriteAsJsonAsync(result);
    }
}