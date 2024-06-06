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
            Detail = exception.Message
        };
        
        switch (exception)
        {
            case TennisClubNotFoundException:
            case SubscriptionTierNotFoundException:
            case MemberNotFoundException:
            case AdminNotFoundException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                break;
            case ConcurrencyException:
            case ConflictException:
            case MemberLimitExceededException:
            case AdminUsernameAlreadyExists:
            case MemberEmailAlreadyExists:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflict";
                break;
            default:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                break;
        }
        
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        
        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}