using Microsoft.AspNetCore.Diagnostics;

namespace PaymentGatewayDemo.Api.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = new HttpErrorResponse(StatusCodes.Status500InternalServerError,
            "An unexpected server error occurred",
            $"Trace id: {httpContext.TraceIdentifier}. Details: {exception.Message}");
        logger.LogError(exception, "An unexpected server error occurred: {ErrorMessage}", exception.Message);

        httpContext.Response.StatusCode = 500;
        await httpContext.Response
            .WriteAsJsonAsync(error, cancellationToken);

        return true;
    }
}