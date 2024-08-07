using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
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
            case WrongPasswordException:
            case AuthenticationException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                break;
            case UnauthorizedAccessException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4";
                problemDetails.Status = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Forbidden";
                break;
            case UserNotFoundException:
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
            case AdminUsernameAlreadyExistsException:
            case MemberEmailAlreadyExistsException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflict";
                break;
            case ValidationException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                break;
            default:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                // Override details to not expose internal error details
                problemDetails.Detail = "Something went wrong.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}