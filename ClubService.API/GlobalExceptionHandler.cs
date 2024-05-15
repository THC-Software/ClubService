using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = exception.GetType().Name,
            Title = "An unhandled error occurred",
            Detail = exception.Message
        };
        
        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}