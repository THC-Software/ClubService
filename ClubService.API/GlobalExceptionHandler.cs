using ClubService.Application.Api.Exceptions;
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
            Type = exception.GetType().Name,
            Detail = exception.Message
        };
        
        switch (exception)
        {
            case TennisClubNotFoundException:
            case SubscriptionTierNotFoundException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = exception.GetType().Name;
                break;
            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                break;
        }
        
        httpContext.Response.StatusCode = (int)problemDetails.Status;
        
        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}