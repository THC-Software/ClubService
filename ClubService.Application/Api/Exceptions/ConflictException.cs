namespace ClubService.Application.Api.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}