namespace ClubService.Application.Api.Exceptions;

public class WrongPasswordException : Exception
{
    public override string Message { get; } = "Wrong Password";
}