using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.Api.Extensions;

internal class GlobalExceptionHandler(
    IHostEnvironment hostingEnvironment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Global exception occurred with message: '{ExceptionMessage}'", exception.Message);

        await WriteHttpResponseAsync(httpContext, exception.Message);

        return true;
    }

    private Task WriteHttpResponseAsync(HttpContext httpContext, string reason)
    {
        var useGenericReason = !hostingEnvironment.IsDevelopment();

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        return httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Title = "InternalServerError",
                Detail = useGenericReason ? "An unexpected error has occurred." : reason,
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = httpContext.Request.Path
            });
    }
}
